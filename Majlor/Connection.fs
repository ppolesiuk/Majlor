namespace Majlor.SMTP

open Majlor
open System.IO
open System.Net.Sockets

type Connection(stream : NetworkStream) as self =
    let writer = new StreamWriter(stream)
    let reader = new StreamReader(stream)
    do
        self.Message 421
        self.Close()

    member this.Message(id : int) =
        writer.Write("{0} ", id);
        writer.Write(Settings.GetSmtpMessage id, Settings.Domain)

    // TODO: linia moze byc za dluga, nalezy rozwazyc taki przypadek
    member this.ReadLine() =
        reader.ReadLine()

    member this.Close() =
        writer.Close()
        reader.Close()
        stream.Close()
