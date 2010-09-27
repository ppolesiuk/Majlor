module Majlor.Program

open System
open System.Net
open System.Net.Sockets

let smtpServe() =
    let server = TcpListener(IPAddress.Any, Settings.Port)
    server.Start()
    while true do
        let client = server.AcceptTcpClient();
        let connection = Majlor.SMTP.Connection client
        ()

let main =
    smtpServe()