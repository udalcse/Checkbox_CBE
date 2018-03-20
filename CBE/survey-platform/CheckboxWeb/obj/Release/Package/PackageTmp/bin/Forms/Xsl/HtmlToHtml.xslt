<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes"/>

  <xsl:include href="LabelledItemToHtml.xslt" />

  <!-- Item -->
  <xsl:template match="instanceData">
    <div style="padding:3px;padding-left:15px;">
      <xsl:value-of select="Html" disable-output-escaping="yes" />
    </div>
  </xsl:template>
</xsl:stylesheet>