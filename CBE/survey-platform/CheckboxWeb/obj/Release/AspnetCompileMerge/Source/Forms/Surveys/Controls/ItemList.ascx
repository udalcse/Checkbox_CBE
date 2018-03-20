<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemList" %>
<%@ Import Namespace="Checkbox.Web"%>

<script type="text/javascript" language="javascript">
    var lastSelected = null;
    $(document).ready(function () {
        $('#itemList').accordion({ icons: '', heightStyle: 'content' });

        $('#itemList .itemSelectorHeader').on({
            mouseenter: function () {
                var selected = $(this);
                lastSelected = selected;

                setTimeout(function () {
                    if (lastSelected != selected)
                        return;

                    var active = $('#itemList .itemSelectorHeader.ui-accordion-header-active');
                    var activeClasses = 'ui-accordion-header-active ui-state-active';
                    if (active != null) {
                        if (active.attr('id') == selected.attr('id'))
                            return;

                        active.removeClass(activeClasses);
                        active.next().removeClass('ui-accordion-content-active').hide('veryslow'); 
                    }

                    selected.addClass(activeClasses).next().addClass('ui-accordion-content-active').show('veryslow');
                }, 500);
            },
            mouseleave: function () {
                lastSelected = null;
            }
        });

        //if there is only one itemtype, lets select it by default
        <% if(DefaultItemInfo != null) { %>
        itemClick('<%= DefaultItemInfo.PreviewImagePath %>', '<%= DefaultItemInfo.Description %>', '<%= DefaultItemInfo.Name %>', '<%= DefaultItemInfo.ExtraData %>');
        <% } %>

        //select on click, highlight on hover
        $('#itemList .itemTypeList p').on({
            click: function () {
                $('#itemList .itemTypeList p').removeClass('selected');
                $(this).addClass('selected');
            },
            mouseenter: function () {
                if (!$(this).hasClass('selected')) {
                    $(this).addClass('hovered');
                }
            },
            mouseleave: function () {
                $(this).removeClass('hovered');
            }
        });
    });
   
    //Show preview and update information panel
    function itemClick(previewUrl, description, typeName, extraData){
        //Update selection
        $('#<%=_selectedItemType.ClientID %>').val(typeName);
        $('#<%=_extraData.ClientID %>').val(extraData);

        //Update preview
        if (previewUrl != null && previewUrl != '') {
            $('#_previewImage').attr('src', previewUrl+"?v=<%=Checkbox.Configuration.Install.ApplicationInstaller.ApplicationAssemblyVersion%>");
            $('#itemPreview').show();
        }
        else {
            $('#itemPreview').hide();
        }
    }
    
</script>

<asp:ObjectDataSource 
    ID="_itemSource" 
    runat="server" 
    SelectMethod="ListItemCategories" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemList" 
    DataObjectTypeName="CheckboxWeb.Forms.Surveys.Controls.ItemList.SimpleItemInfo" 
    OnObjectCreating="OptionsSource_ObjectCreating" />

<div style="display:none;">
    <asp:TextBox ID="_selectedItemType" runat="server" />
    <asp:TextBox ID="_extraData" runat="server" />
</div>

    
<div class="left border999 shadow999 categoryList">
    <asp:ListView ID="_categoryList" runat="server" DataSourceID="_itemSource">
        <LayoutTemplate>
            <div id="itemList">
                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
            </div>
        </LayoutTemplate>    
        <ItemTemplate>
            <h3 class="itemSelectorHeader"><a href="javascript:void(0)"><%# Eval("LocalizedName") %></a></h3>
            <div class="itemListBody">
                <asp:ListView ID="_itemTypeList" runat="server">
                    <LayoutTemplate>
                        <div class="itemTypeList">
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <p><a href="javascript:itemClick('<%# Eval("PreviewImagePath") %>', &quot;<%# Eval("Description") %>&quot;, '<%#Eval("Name") %>', '<%#Eval("ExtraData")%>');"><%# Eval("LocalizedName") %></a></p>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </ItemTemplate>
    </asp:ListView>
    
</div>
<div class="left" style="margin-left:15px;">
        <div class="border999 shadow999" style="width:573px;">
        <div class="dashStatsHeader">
            <ckbx:MultiLanguageLabel ID="_previewLbl" runat="server" TextId="/controlText/forms/surveys/controls/itemList.ascx/preview" CssClass="left mainStats" />
        </div>
    </div>

  
        
    <div id="itemPreview" class="border999 shadow999" style="display:none;margin-top:3px;background-color:White;text-align:center;height:348px;width:572px;">
        <img id="_previewImage" alt='<%=WebTextManager.GetText("/controlText/itemList.ascx/itemPreview") %>' src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/empty.gif") %>" style="max-height:350px;" />
    </div>
</div>