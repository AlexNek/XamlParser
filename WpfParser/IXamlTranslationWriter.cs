namespace WpfParser
{
    public interface IXamlTranslationWriter
    {
        void WriteEntering(XamlNodeBase node);

        void WriteTranslation(XamlAttribute attribute);
    }
}