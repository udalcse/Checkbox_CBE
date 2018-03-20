<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    
    <xsl:param name="noAnswerText" />
    <xsl:output method="text" indent="yes"/>

    <!-- Item -->
    <xsl:template match="/item">
        <xsl:apply-templates select="instanceData/texts" />
        <xsl:apply-templates select="instanceData/answers" />
    </xsl:template>

    <!-- Answers -->
    <xsl:template match="instanceData/answers">
        <xsl:if test="count(answer) = 0">
            <xsl:value-of select="$noAnswerText" />
        </xsl:if>
        <xsl:for-each select="answer">
            <xsl:if test="position() > 1">
                ,
            </xsl:if>
            <xsl:value-of select="." />
        </xsl:for-each>
    </xsl:template>

    <!-- Text -->
    <xsl:template match="instanceData/texts">
        <xsl:if test="text != ''">
            <xsl:value-of select="text"/>
        </xsl:if>
        <xsl:if test="subText != ''">
            <xsl:value-of select="subText"/>
        </xsl:if>
    </xsl:template>
    
</xsl:stylesheet>
