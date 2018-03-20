<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Cache.aspx.cs" Inherits="CheckboxWeb.Settings.Cache" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#cacheAccordion').accordion({
                active: false,
                heightStyle: "content",
                change: function (event, ui) {
                    resizePanels();
                }
            });


        });
    </script>

    <style type="text/css">
        div.cacheItem  {
            font-size:10px;
            padding:5px;
        }
        
        div.cacheValue
        {
            margin-left:15px;
            font-weight:bold;
            float:left;
        }
        
        div.cacheKey
        {
            float:left;
            min-width:350px;
        }
    </style>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <h3>Cache Management</h3>
        <asp:ListView ID="_cacheList" runat="server">
            <LayoutTemplate>
                <div id="cacheAccordion" class="ckbxGrid border999 shadow999" style="width:800px;">
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <h4  class="gridHeader">
                    <a href="#" class="left" style="text-decoration:none;" uframeIgnore="true"><%# Eval("Name") %> (<%#Eval("Count") %>)</a>
                    <btn:CheckboxButton runat="server" ID="_flushButton" CssClass="ckbxButton roundedCorners orangeButton border999 shadow999 right" Text="Flush" style="margin-right:75px;padding-left:10px;padding-right:10px;color:White;"></btn:CheckboxButton>
                    <div class="clear"></div>
                </h4>
                <div>
                    <asp:PlaceHolder ID="_cacheItemPlaceHolder" runat="server" />
                    <div class="clear"></div>
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Content>