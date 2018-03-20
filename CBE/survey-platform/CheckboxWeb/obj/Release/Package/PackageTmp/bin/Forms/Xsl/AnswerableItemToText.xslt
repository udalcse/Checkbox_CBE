<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:param name="noAnswerText" />
    <xsl:output method="text" indent="yes"/>

    <xsl:template match="instanceData/answers">

        <!-- Carriage Return -->
        <xsl:text>&#x0A;</xsl:text>

        <!-- Tab -->
        <xsl:text>&#x09;</xsl:text>
        
        <xsl:if test="count(answer) = 0">
            <xsl:value-of select="$noAnswerText" />
        </xsl:if>
        <xsl:for-each select="answer">
            <xsl:if test="position() > 1">, </xsl:if>
            <xsl:value-of select="." />
        </xsl:for-each>
    </xsl:template>
</xsl:stylesheet>


