namespace Majlor.SMTP

open System.IO
open System.Net.Sockets

type Connection(stream : NetworkStream) =
    let writer = new StreamWriter(stream)
    do
        writer.WriteLine "421 Service not available."
        writer.Close()
        stream.Close()