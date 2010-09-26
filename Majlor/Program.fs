module Majlor.Program

open System
open System.Net
open System.Net.Sockets

let smtpServe() =
    let server = TcpListener(IPAddress.Any, 587)
    server.Start()
    while true do
        let client = server.AcceptTcpClient();
        let connection = Majlor.SMTP.Connection(client.GetStream())
        ()

let main =
    smtpServe()
