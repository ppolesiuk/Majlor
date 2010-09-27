namespace Majlor.SMTP

open Majlor
open Parsor.Combinators
open Parsor.Core
open Parsor.Input
open Parsor.Primitives
open System.IO
open System.Net.Sockets
open System.Threading

type Command =
    | Helo of string
    | Quit
    | SyntaxError
    | Unknown

type Connection(tcpClient : TcpClient) as self =
    //==============================================================
    //                          PARSORS
    //==============================================================

    static let readRestOfLine =
        (
            ((notFollowedBy (skipChar '\r' .>> skipChar '\n') "") >>. getToken) |>
            foldl "" (fun s c -> s + c.ToString())
        ) .>> skipChar '\r' .>> skipChar '\n'
    static let parseInstruction =
        (skipStringIgnoreCase "HELO" <!> fun() ->
            (skipChar ' ' <!> fun() -> (readRestOfLine |>> Helo)) ^|
            (skipChar '\r' >>. skipChar '\n' >>. parsor.Return(Helo ""))
        ) ^|
        (skipStringIgnoreCase "QUIT" <!> fun() -> parsor.Return Quit) ^|
        (skipLine >>. parsor.Return Unknown)

    //==============================================================
    let stream = tcpClient.GetStream()
    let writer = new StreamWriter(stream)
    let mutable clientInput = CharStreamInput stream :> IParsorInput<char>
    let env = Parsor.Environment.NullEnvironment()
    let getInstruction() =
        try
            let (rinp, ins) = parseInstruction(env, clientInput)
            clientInput <- rinp
            ins
        with
        | FatalError _ ->
            let (rinp, _) = skipLine(env, clientInput)
            clientInput <- rinp
            SyntaxError

    let mutable client = ""

    let rec mainState() =
        let ins = getInstruction()
        match ins with
        | Helo cl ->
            client <- cl
            self.Message 250
            mainState()
        | Quit ->
            self.Message 221
            self.Close()
        | SyntaxError ->
            if clientInput.IsEmpty then
                self.Close()
            else
                self.Message 500
                mainState()
        | Unknown ->
            if clientInput.IsEmpty then
                self.Close()
            else
                self.Message 502
                mainState()

    let serverStart() =
        self.Message 220
        mainState()

    do
        let th = Thread(ThreadStart serverStart)
        th.Start()

    member this.Message(id : int) =
        writer.Write("{0} ", id);
        writer.WriteLine(Settings.GetSmtpMessage id, Settings.Domain)
        writer.Flush()

    member this.Close() =
        tcpClient.Client.Disconnect false 
        writer.Close()
        tcpClient.Close()
