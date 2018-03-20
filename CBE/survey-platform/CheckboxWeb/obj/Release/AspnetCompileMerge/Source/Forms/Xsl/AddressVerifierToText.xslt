<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="text" indent="yes"/>

    <xsl:include href="LabelledItemToText.xslt" />
    <xsl:include href="Tokenize.xslt" />

    <xsl:template match="/item">
      <xsl:apply-templates select="instanceData/texts" />

      <xsl:variable name="tokens">
        <xsl:call-template name="tokenize">
          <xsl:with-param name="src" select="instanceData/answers"/>
          <xsl:with-param name="separator" select="'~'"/>
        </xsl:call-template>
      </xsl:variable>
      <xsl:value-of select="msxsl:node-set($tokens)/Token[1]" disable-output-escaping="yes"/>,<xsl:value-of select="msxsl:node-set($tokens)/Token[2]" disable-output-escaping="yes"/>,<xsl:value-of select="msxsl:node-set($tokens)/Token[3]" disable-output-escaping="yes"/>

    </xsl:template>
</xsl:stylesheet>
