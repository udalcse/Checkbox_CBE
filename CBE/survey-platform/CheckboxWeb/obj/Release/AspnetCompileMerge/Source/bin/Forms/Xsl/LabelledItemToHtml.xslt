<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

    <xsl:output method="html" indent="yes"/>

    <xsl:template match="instanceData/texts">
      <div>
        <xsl:if test="text != ''">
            <div class="Question">
                <xsl:value-of select="text" disable-output-escaping="yes" />
            </div>
        </xsl:if>
        <xsl:if test="subText != ''">
            <div class="Description">
                <xsl:value-of select="subText" disable-output-escaping="yes" />
            </div>
        </xsl:if>
      </div>
    </xsl:template>
</xsl:stylesheet>