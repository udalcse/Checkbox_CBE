<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

    <xsl:output method="text" indent="yes" />

    <xsl:template match="instanceData/texts">
        <xsl:if test="text != ''">
            <!-- Carriage Return -->
            <xsl:text>&#x0A;</xsl:text>
            <xsl:value-of select="text" disable-output-escaping="yes" />
        </xsl:if>
        <xsl:if test="subText != ''">
            <!-- Carriage Return -->
            <xsl:text>&#x0A;</xsl:text>
            <xsl:value-of select="subText" disable-output-escaping="yes" />
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
