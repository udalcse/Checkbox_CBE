<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.ImageSelector" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#imageSelectorTabs_<%=ID %>').ckbxTabs({
                tabName: 'imageSelectorTabs_<%=ID %>',
                initialTabIndex:<%=_currentEditMode.Text %>,
                tabStyle: 'inverted',
                onTabClick: function (index) { $('#<%=_currentEditMode.ClientID %>').val(index); },
                onTabsLoaded: function () { $('#imageSelectorTabs_<%=ID %>').show(); $('.tabContentContainer').show(); }
            });
        });
        
         //Handle file selection.
        function onImageFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
                $('#<%=_fileUploadedBtn.ClientID %>').click();
            }
        }
    </script>

    <div style="display:none">
        <asp:TextBox ID="_currentEditMode" runat="server"></asp:TextBox>
        <asp:TextBox ID="_uploadedImageId" runat="server"></asp:TextBox>
        <asp:Button ID="_fileUploadedBtn" runat="server" Text="Uploaded" />
        <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
        <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
    </div>

     <ul id="imageSelectorTabs_<%=ID %>" class="tabContainer">
        <li><%=WebTextManager.GetText("/controlText/imageSelector/upload")%></li>
        <li><%= WebTextManager.GetText("/controlText/imageSelector/fromWebSite") %></li>
        <li><%=WebTextManager.GetText("/controlText/imageSelector/previouslyUploaded")%></li>
    </ul>
    <div class="clear"></div>

    <div class="padding10">
        <div class="tabContentContainer">
            <div id="imageSelectorTabs_<%=ID %>-0-tabContent" class="padding10">
                <div>
                    <br class="clear" />
                        <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="onImageFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
                    <br class="clear" />
                </div>
            </div>
             <div id="imageSelectorTabs_<%=ID %>-1-tabContent" class="padding10">    
                    <asp:Panel ID="_fileUploadedPanel" runat="server">
                        <p>
                            <ckbx:MultiLanguageLabel ID="_fileUploadedLbl" runat="server" TextId="/controlText/imageSelector/fileUploaded" />
                        </p>
                    </asp:Panel>
            
                    <div class="formInput">
                        <p><ckbx:MultiLanguageLabel AssociatedControlID="_imageUrlTxt" ID="_imageUrlLbl" runat="server" TextId="/controlText/imageSelector/imageUrl" /></p>
                        <div style="float:left;">
                            <asp:TextBox ID="_imageUrlTxt" runat="server" Width="400" />
                        </div>
                        <div style="float:left;margin-top:2px;margin-left:10px;">
                            <btn:CheckboxButton CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" ID="_updatePreviewBtn" runat="server" TextId="/controlText/imageSelector/updatePreview" />
                        </div>
                        <br class="clear" />
                    </div>
            
                    <div style="margin-top:20px;">
                        <asp:Panel ID="_previewMsgPanel" runat="server" CssClass="dialogSubTitle">
                            <%=WebTextManager.GetText("/controlText/imageSelector/preview")%>
                        </asp:Panel>

                        <asp:Panel ID="_noPreviewMsgPanel" runat="server" CssClass="formInput">
                            <p><label><%=WebTextManager.GetText("/controlText/imageSelector/noPreview")%></label></p>
                        </asp:Panel>
                    </div>
             </div>
            <div id="imageSelectorTabs_<%=ID %>-2-tabContent" class="padding10">
                <div class="formInput">
                    <p><ckbx:MultiLanguageLabel ID="_imageLbl" runat="server" AssociatedControlID="_imageList" TextId="/controlText/imageSelector/imageName" /></p>
                    <asp:DropDownList ID="_imageList" runat="server" AutoPostBack="true" />
                </div>
            </div>
           
            <asp:Panel ID="_previewPanel" runat="server" CssClass="roundedCorners border999 shadow999" style="width:550px;height:225px;overflow:auto;">
                <asp:Image ID="_previewImage" runat="server" />
            </asp:Panel>
        </div>
    </div>