<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:param name="enableScoring" />
    <xsl:output method="html" indent="yes"/>

    <xsl:include href="LabelledItemToHtml.xslt" />

    <xsl:template match="/item">
        <div style="padding-top:5px;">
            <xsl:apply-templates select="instanceData/texts" />
            <xsl:apply-templates select="instanceData/answers" />
        </div>
    </xsl:template>

    <xsl:template match="instanceData/answers">
        <div class="Answer">
            <xsl:for-each select="answer">
                <xsl:if test="position() > 1">
                    <br />
                </xsl:if>
                <xsl:if test="number(.) &gt;= 0 and number(.) &lt;= 6">
                   <span class="nps-detractor">Detractor</span>
                </xsl:if>
                <xsl:if test="number(.) &gt;= 7 and number(.) &lt;= 8">
                  <span class="nps-passive">Passive</span>
                </xsl:if>
                <xsl:if test="number(.) &gt;= 9 and number(.) &lt;= 10">
                  <span class="nps-promoter">Promoter</span>
                </xsl:if>
                (<xsl:value-of select="." disable-output-escaping="yes" />)
                <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                <xsl:if test=". != '0'">
                    <xsl:variable name="answerOptionId" select="@optionId" />
                        <xsl:if test="$enableScoring = 'True'">
                           (<xsl:value-of select="../../listOptions/listOption[@optionId=$answerOptionId]/points" />)
                        </xsl:if>
                </xsl:if>
            </xsl:for-each>
        </div>
    </xsl:template>
</xsl:stylesheet>
