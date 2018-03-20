<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="html" indent="yes"/>

    <xsl:include href="LabelledItemToHtml.xslt" />

    <xsl:template match="/item">
        <div>
            <xsl:apply-templates select="instanceData/texts" />
            <xsl:apply-templates select="instanceData/answers" />
        </div>
    </xsl:template>

    <xsl:template match="instanceData/answers">
        <div class="Answer left">
			<table>
			  <xsl:for-each select="answer">
				<tr>
				<td>					
					<xsl:if test="@imageUrl != ''">					
						<xsl:text disable-output-escaping="yes"><![CDATA[<img src=']]></xsl:text>			  
						<xsl:value-of select="@imageUrl" />
						<xsl:text disable-output-escaping="yes"><![CDATA[' />]]></xsl:text>		
						<xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
				    </xsl:if>
					<xsl:value-of select="." disable-output-escaping="yes" />					
				</td>
				<td>
					<xsl:variable name="answerOptionId" select="@optionId" />
					<xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
					<xsl:value-of select="../../listOptions/listOption[@optionId=$answerOptionId]/points" />					
				</td>
				</tr>
			  </xsl:for-each>
			</table>
        </div>
      <br class="clear" />
    </xsl:template>
</xsl:stylesheet>
