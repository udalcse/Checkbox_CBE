<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    
    <xsl:param name="noAnswerText" />
    <xsl:output method="html" indent="yes"/>

    <!-- Item -->
    <xsl:template match="/item">
        <div>
            <xsl:apply-templates select="instanceData/texts" />
            <xsl:apply-templates select="instanceData/answers" />
        </div>
    </xsl:template>

    <!-- Answers -->
    <xsl:template match="instanceData/answers">
        <div class="Answer">
            <xsl:if test="count(answer) = 0">
                <xsl:value-of select="$noAnswerText" />
            </xsl:if>
            <xsl:for-each select="answer">
                <xsl:if test="position() > 1">
                    ,
                </xsl:if>
                <xsl:value-of select="." />
            </xsl:for-each>
        </div>
    </xsl:template>

    <!-- Text -->
    <xsl:template match="instanceData/texts">
        <div>
            <xsl:if test="text != ''">
                <div class="Question">
                    <xsl:value-of select="text"/>
                </div>
            </xsl:if>
            <xsl:if test="subText != ''">
                <div class="Description">
                    <xsl:value-of select="subText"/>
                </div>
            </xsl:if>
        </div>
    </xsl:template>
    
</xsl:stylesheet>
