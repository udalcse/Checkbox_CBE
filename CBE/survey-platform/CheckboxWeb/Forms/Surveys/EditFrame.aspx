<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="EditFrame.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.EditFrame" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" ID="_ckbxEdit" Source="~/Resources/jquery.ckbxEditable.js" />

    <script type="text/javascript">
        var _appRoot = '<%=ResolveUrl("~") %>';
        $(document).ready(function () {
            
            $('#surveyHeader').ckbxEditable({
                textAreaColumns:120,
                textAreaRows: 10,
                appRoot:_appRoot
            });

            $('#surveyFooter').ckbxEditable({
                textAreaColumns: 120,
                textAreaRows: 10,
                appRoot: _appRoot
            });

            $('div[settingName]').ckbxEditable({
                appRoot: _appRoot
            });
        });
    </script>

     <style type="text/css">
        #toolbox
        {
            padding:10px;
            width:150px;
            height:200px;
            position:fixed;
            border-radius:10px;
            border-style:solid;
            border-width:1px 3px 3px 1px;
            background-color:white;
        }
        
        #editor
        {
            margin-left:180px;
            width:1050px;
            height:700px;
            overflow:auto;
            border-style:solid;
            border-width:1px 3px 3px 1px;
            background-color:white;
            margin-top:5px;
            border-radius:10px;
            padding:10px;
            position:relative;
        }
        
        #surveyFooterContainer
        {
            position:absolute;
            bottom:0;
            margin-left:auto;
            margin-right:auto;
            min-height:100px;
            width: 1050px;
            border:1px solid red;
        }
        
        #surveyHeaderContainer
        {
            min-height:100px;
            border:1px solid green;
        }
    </style>

</asp:Content>

<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">

    <div id="toolbox">
        [TOOLBOX]
    </div>
    <div id="editor">
        <div id="surveyHeaderContainer">
            <div id="surveyHeader" editMode="tinyMCE">
                <div style="text-align: center;">
                    <i>Click here to edit your survey header</i>
                </div>
            </div>
        </div>
        <div id="surveyTitle" settingName="surveyTitle">
            <%=ResponseTemplate.Name %>
        </div>
         <div id="Div1" settingName="surveyTitle">
            <%=ResponseTemplate.Name %>
        </div>
       
        <div id="surveyFooterContainer">
            <div id="surveyFooter" editMode="tinyMCE">
                <div style="text-align: center;margin-top:auto;margin-bottom:auto;">
                    <i>Click here to edit your survey footer</i>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
