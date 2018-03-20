<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScoreStatisticsTable.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.NetPromoterScoreStatisticsTable" %>

<style runat="server" type="text/css" id="_dataControlStyles">
     /* Container for whole renderer */
     .itemContainer
     {
        clear:both;
     }

     /* Container for a row of the output */
    .rowDiv
    {
        clear:both;
        height:25px;
	    position:relative;
    }
    
     /* Bar showing average value */
	.bar
	{
	    background-color:blue;
	    height:20px;
	    border:1px solid black;
	    float:left;
	    margin-top:3px;
	    margin-bottom:3px;
    }
	
	 /* Text displayed in a row */
	.rowText
	{
	    width:100px;
	    float:left;
	    border-bottom:1px solid black;
	    border-left:1px solid black;
	    border-right:1px solid black;
	    text-align:center;
	    height:25px;
	    overflow:visible;
	    line-height:25px;
	    font-family:Arial;
        font-size:10px;
    }
    
    .questionCell
    {
        width:250px !important;
    }
    
    
    .verticalGridLine
    {
        height:25px;
        width:1px;
        background-color:#AAAAAA;
        overflow:hidden;
        float:left;
        position:absolute;
        top:2px;
    }
    
     /* Text displayed in a row */
	.rowHeaderText
	{
	    width:100px;
	    float:left;
	    background: lightgray;
	    border-top:1px solid black;
	    border-bottom:1px solid black;
	    border-right:1px solid black;
	    text-align:center;
	    height:25px;
	    overflow:visible;
	    line-height:25px;
    }
	
	 /* Labels displayed in header */
	.numberLabel
	{
	    width:60px;
	    float:left;
	    border:0;
	    text-align:center;
	    height:25px;
	    line-height:25px;
	    border-top:1px solid black;
	    border-bottom:1px solid black;
        font-family:Arial;
        font-size:10px;
    }
	
	 /* Spacer div placed between bar and avg/median/std.dev text elements. */
	.spacer
	{
	    float:left;
	    height:25px;
    }
	
     /* Cap on end of error bar */
	.errorBarCap
	{
	    float:left;
	    position:absolute;
	    height:8px;
	    background-color:green;
	    width:2px;
	    margin-top:10px;
    }
	
	 /* Error bar */
	.errorBar
	{
	    margin-top:13px;
	    background-color:green;
	    float:left;
	    position:absolute;
	    height:2px;
    }

</style>

<div class="itemContent pageBreak" style="width: 825px; margin:30px auto;">
    <table class="matrix Matrix" cellspacing="0" cellpadding="2" rules="all" border="1" style="width: 825px; border-collapse:collapse;margin-left:auto;margin-right:auto;">
    	<tbody>
            <tr class="header">
        		<th align="center" scope="col">                
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/question" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colMinHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/min" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colMaxHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/max" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colAverageHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/average" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colVarianceHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/variance" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colStdDevHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/standardDeviation" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colResponsesHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/totalResponses" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colRespondentsHeader">
                        <ckbx:MultiLanguageLabel TextID="/controlText/netPromoterScoreStatisticsItemRenderer/totalRespondents" runat="server"/>
                </th>
			</tr>            
        <asp:Repeater ID="_rowRepeater" runat="server">
            <ItemTemplate>
                <tr class="<%# Container.ItemIndex % 2 == 0 ? "Item" : "AlternatingItem" %>">
				    <td align="left">
                        <asp:Label Text="<%#GetItemText((int)Container.DataItem)%>" runat="server"/>
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowMin%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "MinValue").ToString()%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowMax%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "MaxValue").ToString()%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowAverage%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "AverageValue").ToString("0.0#")%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowVariance%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "Variance").ToString("0.0#")%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server"  Visible="<%#ShowStandartDeviation%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "StandardDeviation").ToString("0.0#")%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowTotalResponses%>" >
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "TotalResponses").ToString()%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowTotalRespondents%>">
                        <asp:Label Text='<%#GetValue((int)Container.DataItem, "TotalRespondents").ToString()%>' runat="server" />                        
                    </td>
			    </tr>
            </ItemTemplate>
        </asp:Repeater>
		</tbody>
    </table>
</div>
