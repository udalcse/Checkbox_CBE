<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text" indent="yes"/>

  <xsl:include href="LabelledItemToText.xslt" />

  <!-- Item -->
  <xsl:template match="instanceData">
    <xsl:value-of select="Text" disable-output-escaping="yes" />
  </xsl:template>
</xsl:stylesheet>