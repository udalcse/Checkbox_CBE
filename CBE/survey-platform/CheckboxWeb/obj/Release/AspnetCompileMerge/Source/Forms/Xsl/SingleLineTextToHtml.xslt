<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="html" indent="yes"/>

    <xsl:include href="LabelledItemToHtml.xslt" />
    <xsl:include href="AnswerableItemToHtml.xslt" />

    <xsl:template match="/item">
        <div style="padding-top:5px;">
            <xsl:apply-templates select="instanceData/texts" />
            <xsl:apply-templates select="instanceData/answers" />
        </div>
    </xsl:template>
</xsl:stylesheet>
