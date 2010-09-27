namespace Majlor

// TODO : ustawienia przenieść do pliku XML, albo czegokolwiek
type Settings() =
    static let smtpMessages =
        let dict = System.Collections.Generic.Dictionary<int, string>()
        dict.Add(220, "{0} Service ready")
        dict.Add(421, "{0} Service not available")
        dict
    static member Domain = "127.0.0.1"
    static member Port = 587

    static member GetSmtpMessage id =
        smtpMessages.[id]
