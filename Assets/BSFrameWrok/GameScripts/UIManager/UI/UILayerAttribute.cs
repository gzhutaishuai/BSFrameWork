using System;

namespace Core.UI
{
public class UILayerAttribute :Attribute
{
    public UIPanelLayer layer { get; }

    /// <summary>
    /// 指定层级面板，通过映射的layer来确定UI层级
    /// </summary>
    /// <param name="layer"></param>
    public UILayerAttribute(UIPanelLayer layer)
    {
        this.layer = layer;
    }
}
}