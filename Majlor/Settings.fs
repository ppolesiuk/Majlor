namespace Majlor

// TODO : ustawienia przenieść do pliku XML, albo czegokolwiek
type Settings() =
    static let smtpMessages =
        let dict = System.Collections.Generic.Dictionary<int, string>()
        dict.Add(220, "{0} Service ready")
        dict.Add(221, "{0} Service closing transmission channel")
        dict.Add(250, "Requested mail action okay, completed")
        dict.Add(421, "{0} Service not available")
        dict.Add(500, "Syntax error")
        dict.Add(502, "Command not implemented")
        dict
    static member Domain = "127.0.0.1"
    static member Port = 587

    static member GetSmtpMessage id =
        smtpMessages.[id]
