using System.Collections.Generic;

namespace WpfParser
{
    public class XamlCodeTextVisitor : IXamlNodeVisitor
    {
        private static HashSet<string> _DisableAttrNameMap;

        private static HashSet<string> _DisableObjMap;

        private readonly IXamlTranslationWriter _translationWriter;

        public XamlCodeTextVisitor(IXamlTranslationWriter translationWriter)
        {
            _translationWriter = translationWriter;
        }

        public bool IsAllowedToVisit(XamlNodeBase node)
        {
            if (node == null)
            {
                return false;
            }

            return node.NodeType == XamlNodeBase.ENodeType.Attribute;
        }

        public void Visit(XamlNodeBase node, XamlNodeWalker xamlNodeWalker)
        {
            if (_DisableObjMap == null)
            {
                _DisableObjMap = new HashSet<string>();
                _DisableObjMap.Add("Image");
                _DisableObjMap.Add("StackPanel");
                _DisableObjMap.Add("DockPanel");
                _DisableObjMap.Add("EventToCommand");
                _DisableObjMap.Add("GradientStop");
                _DisableObjMap.Add("LinearGradientBrush");
                _DisableObjMap.Add("RenderOptions");
                _DisableObjMap.Add("Rectangle");
                _DisableObjMap.Add("ViewModelSource");
                _DisableObjMap.Add("Trigger");
                _DisableObjMap.Add("ScrollViewer");
            }

            if (_DisableAttrNameMap == null)
            {
                _DisableAttrNameMap = new HashSet<string>();
                _DisableAttrNameMap.Add("AllowBestFit");
                _DisableAttrNameMap.Add("AllowEditing");
                _DisableAttrNameMap.Add("AllowGroup");
                _DisableAttrNameMap.Add("AllowSort");
                _DisableAttrNameMap.Add("AreHeadersSticky");
                _DisableAttrNameMap.Add("AutoCreateColumns");
                _DisableAttrNameMap.Add("AutoGenerateColumns");
                _DisableAttrNameMap.Add("AutoSizeMode");
                _DisableAttrNameMap.Add("AutoWidth");
                _DisableAttrNameMap.Add("Background");
                _DisableAttrNameMap.Add("BorderThickness");
                _DisableAttrNameMap.Add("BorderBrush");
                _DisableAttrNameMap.Add("CanBeCurrentWhenReadOnly");
                _DisableAttrNameMap.Add("CellHorizontalContentAlignment");
                _DisableAttrNameMap.Add("CellVerticalContentAlignment");
                _DisableAttrNameMap.Add("ContainerHeight");
                _DisableAttrNameMap.Add("CornerRadius");
                _DisableAttrNameMap.Add("ContentAlignment");
                _DisableAttrNameMap.Add("DataNavigatorButtons");
                _DisableAttrNameMap.Add("DispatcherPriority");
                _DisableAttrNameMap.Add("DockingStyle");
                _DisableAttrNameMap.Add("DragEnter");
                _DisableAttrNameMap.Add("FieldName");
                _DisableAttrNameMap.Add("FontSize");
                _DisableAttrNameMap.Add("Height");
                _DisableAttrNameMap.Add("HorizontalAlignment");
                _DisableAttrNameMap.Add("HorizontalContentAlignment");
                _DisableAttrNameMap.Add("IsCopyCommandEnabled");
                _DisableAttrNameMap.Add("Icon");
                _DisableAttrNameMap.Add("IsDefault");
                _DisableAttrNameMap.Add("IsFocused");
                _DisableAttrNameMap.Add("IsManipulationEnabled");
                _DisableAttrNameMap.Add("IsReadOnly");
                _DisableAttrNameMap.Add("ItemHeight");
                _DisableAttrNameMap.Add("ItemsSourcePath");
                _DisableAttrNameMap.Add("ItemWidth");
                _DisableAttrNameMap.Add("Margin");
                _DisableAttrNameMap.Add("MaxWidth");
                _DisableAttrNameMap.Add("MinLines");
                _DisableAttrNameMap.Add("MinWidth");
                _DisableAttrNameMap.Add("Loaded");
                _DisableAttrNameMap.Add("NavigationBehavior");
                _DisableAttrNameMap.Add("Orientation");
                _DisableAttrNameMap.Add("Padding");
                _DisableAttrNameMap.Add("Property");
                _DisableAttrNameMap.Add("ReadOnly");
                _DisableAttrNameMap.Add("RecognizesAccessKey");
                _DisableAttrNameMap.Add("Row");
                _DisableAttrNameMap.Add("SelectionChanged");
                _DisableAttrNameMap.Add("ShowColumnHeaders");
                _DisableAttrNameMap.Add("ShowDataNavigator");
                _DisableAttrNameMap.Add("ShowGroupPanel");
                _DisableAttrNameMap.Add("StartupUri");
                _DisableAttrNameMap.Add("TabStripPlacement");
                _DisableAttrNameMap.Add("TargetType");
                _DisableAttrNameMap.Add("ThemeName");
                _DisableAttrNameMap.Add("TitleAlignment");
                _DisableAttrNameMap.Add("UnboundType");
                _DisableAttrNameMap.Add("UseLayoutRounding");
                _DisableAttrNameMap.Add("VerticalAlignment");
                _DisableAttrNameMap.Add("VerticalContentAlignment");
                _DisableAttrNameMap.Add("VerticalGridLineBrush");
                _DisableAttrNameMap.Add("VerticalGridLineThickness");
                _DisableAttrNameMap.Add("VerticalScrollBarVisibility");
                _DisableAttrNameMap.Add("Visibility");
                _DisableAttrNameMap.Add("Width");
                _DisableAttrNameMap.Add("WindowStartupLocation");
                _DisableAttrNameMap.Add("WindowState");
            }

            _translationWriter.WriteEntering(node);

            if (node is XamlAttribute attribute)
            {
                if (attribute.Value != null && attribute.Value is string && attribute.DeclaringType != null)
                {
                    if (!_DisableObjMap.Contains(attribute.DeclaringType.Name) && !_DisableAttrNameMap.Contains(attribute.Name))
                    {
                        bool isAllowed = true;
                        if (attribute.Parent is XamlObjectNode parent)
                        {
                            if (parent.XamlType.Name == "Setter" || parent.XamlType.Name == "Condition")
                            {
                                XamlAttribute parentAttribute = parent.Attributes[0];
                                if (parentAttribute.Name == "Property")
                                {
                                    string parentAttributeValue = parentAttribute.Value.ToString();
                                    if (_DisableAttrNameMap.Contains(parentAttributeValue))
                                    {
                                        isAllowed = false;
                                    }
                                }
                            }
                        }

                        if (isAllowed)
                        {
                            _translationWriter.WriteTranslation(attribute);
                        }
                    }
                }
            }
        }
    }
}
