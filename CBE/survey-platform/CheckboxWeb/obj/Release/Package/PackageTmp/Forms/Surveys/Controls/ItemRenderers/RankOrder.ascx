<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrder.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RankOrder" %>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/RankOrderNumeric.ascx" TagName="RankOrderNumeric" TagPrefix="rod" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/RankOrderTopN.ascx" TagName="RankOrderTopN" TagPrefix="rod" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/RankOrderDragNDrop.ascx" TagName="RankOrderDragNDrop" TagPrefix="rod" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/RankOrderSelectableDragnDrop.ascx" TagName="RankOrderSelectableDragnDrop" TagPrefix="rod" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.ui.touch-punch.min.js" ></ckbx:ResolvingScriptElement>
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.numeric.js" ></ckbx:ResolvingScriptElement>

    <script type="text/javascript">
        $(function () {
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {

                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');

                if (question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin + 'px');
                }
            }
        });        
    </script>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <rod:RankOrderNumeric ID="_rankOrderNumeric" runat="server" />
                <rod:RankOrderTopN ID="_rankOrderTopN" runat="server" />
                <rod:RankOrderDragNDrop ID="_rankOrderDragNDrop" runat="server" />
                <rod:RankOrderSelectableDragnDrop ID="_selectableDragnDrop" runat="server" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>


<script type="text/C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var childControls = base.ChildUserControls;
            childControls.Add(_questionText);
            return childControls;
        }
    }

    /// <summary>
    /// Get/Set Current rank order
    /// </summary>
    protected RankOrderBase CurrentRankOrder { get; set; }

    /// <summary>
    /// Get rankOrder type
    /// </summary>
    protected RankOrderType RankOrderType
    {
        get
        {
            if (ExportMode == ExportMode.Pdf)
                return RankOrderType.Numeric;
            
            return (RankOrderType) Enum.Parse(typeof (RankOrderType), Model.InstanceData["RankOrderType"]);
        }
    } 
    
    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        CurrentRankOrder = null;
        
        switch (RankOrderType)
        {
            case RankOrderType.SelectableDragnDrop:
                CurrentRankOrder = _selectableDragnDrop;
                break;
            case RankOrderType.Numeric:
                CurrentRankOrder = _rankOrderNumeric;
                break;
            case RankOrderType.TopN:
                CurrentRankOrder = _rankOrderTopN;
                break;
            case RankOrderType.DragnDroppable:
                CurrentRankOrder = _rankOrderDragNDrop;
                break;
        }
        
        SetVisibility();
        SetLabelPosition();
        SetItemPosition();
        
        CurrentRankOrder.Initialize(Model);      
    }

    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        CurrentRankOrder.BindModel();
    }

    /// <summary>
    /// Set rankOrders visibility
    /// </summary>
    private void SetVisibility()
    {
        _rankOrderTopN.Visible = _rankOrderNumeric.Visible = _rankOrderDragNDrop.Visible = _selectableDragnDrop.Visible = false;
        CurrentRankOrder.Visible = true;
    }
   

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        UpsertOptionAnswers(CurrentRankOrder.GetOptionsWithPoints());
    }

    /// <summary>
    /// Reorganize controls and/or apply specific styles depending
    /// on item's label position setting.
    /// </summary>
    protected void SetLabelPosition()
    {
        //When label is set to bottom, we need to move controls from the top panel
        // to the bottom panel.  Otherwise, position changes are managed by setting
        // CSS class.
        if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            //Move text controls to bottom
            _bottomAndOrRightPanel.Controls.Add(_textContainer);

            //Move input to top
            _topAndOrLeftPanel.Controls.Add(_inputPanel);
        }

        //Set css Top
        _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        _bottomAndOrRightPanel.CssClass = "rank-order-bottom-right-container bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Top");
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

        if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        }

        _bottomAndOrRightPanel.Style[HtmlTextWriterStyle.PaddingBottom] = "15px";
    }

</script>