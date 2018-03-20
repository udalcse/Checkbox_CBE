<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="logEntry">
		<hr size="1" />
		<p><div class="PrezzaSubTitle" style="background-color:#a10008;color:white;padding:2px">Basic Information</div></p>
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<tr>
				<td class="EvenRow" width="100px"><b>TimeStamp:</b></td>
				<td class="EvenRow"><xsl:value-of select="timestamp" /></td>
			</tr>
			<tr>
				<td class="OddRow" width="100px"><b>Exception ID:</b></td>
				<td class="OddRow"><xsl:value-of select="message/Exception/ExceptionId" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Description:</b></td>
				<td class="EvenRow"><xsl:value-of select="message/Exception/Description" /></td>
			</tr>
			<tr>
				<td class="OddRow" width="100px"><b>Message:</b></td>
				<td class="OddRow"><xsl:value-of select="message/Exception/Message" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Exception Type:</b></td>
				<td class="EvenRow"><xsl:value-of select="message/Exception/ExceptionType" /></td>
			</tr>
			<tr>
				<td class="OddRow" width="100px"><b>Source:</b></td>
				<td class="OddRow"><xsl:value-of select="message/Exception/Source" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Help Link:</b></td>
				<td class="EvenRow">
					<a>
						<xsl:attribute name="href"><xsl:value-of select="message/Exception/HelpLink" /></xsl:attribute>
						<xsl:value-of select="message/Exception/HelpLink" />
					</a>
				</td>
			</tr>
		</table>
		
		<hr size="1" />
		
		<p><div class="PrezzaSubTitle" style="background-color:#a10008;color:white;padding:2px">Properties</div></p>
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<xsl:apply-templates select="message/Exception/Property" />
		</table>
		
		<hr size="1" />
		
		<p><div class="PrezzaSubTitle" style="background-color:#a10008;color:white;padding:2px">Additional Information</div></p>
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<xsl:apply-templates select="message/Exception/AdditionalInfo/info" />
		</table>
		
		<hr size="1" />
		
		<p><div class="PrezzaSubTitle" style="background-color:#a10008;color:white;padding:2px">Stack Trace</div></p>
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<xsl:apply-templates select="message/Exception/StackTrace" />
		</table>
		
		<hr size="1" />
		
		<p><div class="PrezzaSubTitle" style="background-color:#a10008;color:white;padding:2px">Inner Exception(s)</div></p>
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<tr>
				<td width="15px"></td>
				<td><xsl:apply-templates select="message/Exception/InnerException" /></td>
			</tr>
		</table>

		
	</xsl:template>
	
	<xsl:template match="Property">
		<tr>
			<td width="100px"><b><xsl:value-of select="@name" /></b></td>
			<td><xsl:value-of select="." /></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="AdditionalInfo/info">
		<tr>
			<td width="100px"><b><xsl:value-of select="@name" /></b></td>
			<td><xsl:value-of select="@value" /></td>
		</tr>
	</xsl:template>
	
	<xsl:template match="StackTrace">
		<span style="font: 12px Courier New;"><xsl:value-of select="." /></span>
	</xsl:template>
	
	<xsl:template match="InnerException">
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<tr>
				<td class="EvenRow" width="100px"><b>ExceptionType:</b></td>
				<td class="EvenRow"><xsl:value-of select="ExceptionType" /></td>
			</tr>
			<tr>
				<td class="OddRow" width="100px"><b>Message:</b></td>
				<td class="OddRow"><xsl:value-of select="Message" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Source:</b></td>
				<td class="EvenRow"><xsl:value-of select="Source" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Help Link:</b></td>
				<td class="EvenRow"><xsl:value-of select="HelpLink" /></td>
			</tr>
			<tr>
				<td class="EvenRow" width="100px"><b>Stack Trace:</b></td>
				<td class="EvenRow"><xsl:value-of select="StackTrace" /></td>
			</tr>
		</table>
		<br />
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<xsl:apply-templates select="Property" />
		</table>
		<br />
		<table cellspacing="1" cellpadding="2" background-color="#0A0A0A" class="PrezzaDataGrid" width="100%">
			<tr>
				<td style="background-color:#0039b4;padding:4px;color:white" colspan="2">Inner Exception</td>
			</tr>
			<tr>
				<td width="15px"></td>
				<td><xsl:apply-templates select="InnerException" /></td>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet>

  