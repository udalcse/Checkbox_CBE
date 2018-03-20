<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <!-- splits string into <token> elements -->
  <xsl:template name="tokenize">
    <xsl:param name="src"/>
    <xsl:param name="separator"/>
    <xsl:choose>
      <xsl:when test="contains($src, $separator)">
        
        <!-- build first token element -->
        <xsl:element name="Token">
          <xsl:value-of
          select="substring-before($src, $separator)"/>
          
          </xsl:element>
        
          <!-- recurse -->
          <xsl:call-template name="tokenize">
            <xsl:with-param name="src" select="substring-after($src, $separator)"/>
            <xsl:with-param name="separator" select="$separator"/>
          </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <!-- last token, end recursion -->
        <xsl:element name="Token">
          <xsl:value-of select="$src"/>
        </xsl:element>
      </xsl:otherwise>
      
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
