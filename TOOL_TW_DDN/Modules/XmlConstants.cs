public static class XmlConstants
{
    public static readonly string XmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                                              "<!DOCTYPE raml SYSTEM 'raml20.dtd'>\n" +
                                              "<raml version=\"2.0\" xmlns=\"raml20.xsd\">\n" +
                                              "<cmData type=\"plan\">\n" +
                                              "<header>\n <log dateTime=\"\" action=\"created\" appInfo=\"PlanExporter\">UIValues are used</log>\n</header>";
    public static readonly string XmlFooter = "</cmData>\n</raml>";
}