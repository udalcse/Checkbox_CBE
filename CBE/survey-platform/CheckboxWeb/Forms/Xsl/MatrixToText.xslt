<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="text" indent="yes"/>

    <xsl:include href="LabelledItemToText.xslt" />
    <xsl:include href="AnswerableItemToText.xslt" />

    <!-- Item template -->
    <xsl:template match="/item">
        <xsl:variable name="pkIndex" select="metaData/matrixMetaData/pkColumnIndex" />

            <!-- Matrix Top-Level Question -->
            <xsl:apply-templates select="instanceData/texts" />

            <!-- Matrix-->
                <xsl:for-each select="instanceData/rows/row">
                    <xsl:if test="position() mod 2 = 0">
                        <xsl:apply-templates select=".">
                            <xsl:with-param name="rowStyle">Item</xsl:with-param>
                            <xsl:with-param name="pkIndex" select="$pkIndex" />
                        </xsl:apply-templates>
                    </xsl:if>
                    <xsl:if test="position() mod 2 != 0">
                        <xsl:apply-templates select=".">
                            <xsl:with-param name="rowStyle">AlternatingItem</xsl:with-param>
                            <xsl:with-param name="pkIndex" select="$pkIndex" />
                        </xsl:apply-templates>
                    </xsl:if>
                </xsl:for-each>
    </xsl:template>

    <!-- Row Template -->
    <xsl:template match="row">
        <xsl:param name="rowStyle" />
        <xsl:param name="pkIndex" />

        <xsl:variable name="rowType" select="@rowType "/>

        <xsl:variable name="rowText" select="text" />

        <!-- Ignore subheadings -->
        <xsl:if test="$rowType != 'Subheading'">
            <!-- Carriage Return -->
            <xsl:text>&#x0A;</xsl:text>
            <xsl:text>&#x09;</xsl:text>

            <!-- Normal Row Text -->
            <xsl:if test="$rowType = 'Normal'">
                <xsl:value-of select="$rowText" />
            </xsl:if>

            <!-- Other Row Answer -->
            <xsl:if test="$rowType = 'Other'">
                <xsl:value-of select="columns/column[@isPkColumn='true']/item/instanceData/answers/answer" />
            </xsl:if>


            <!-- CR & Tab -->
            <xsl:text>&#x0A;</xsl:text>
            <xsl:text>&#x09;</xsl:text>
            <xsl:text>&#x09;</xsl:text>
            
            <!-- Add columns, with special handling for pk column-->
            <xsl:for-each select="columns/column">
                <!-- Ignore PK Column -->
                <xsl:if test="position() != $pkIndex">
                    <!-- Column Text -->
                    <xsl:value-of select="./text" disable-output-escaping="yes" />
                    
                    <!-- Separator -->
                    <xsl:text> -- </xsl:text>

                    <!-- Column Answers -->
                    <xsl:if test="position() != $pkIndex">
                        <xsl:for-each select="item/instanceData/answers/answer">
                            <xsl:if test="position() > 1">, </xsl:if>
                            <xsl:value-of select="." />
                        </xsl:for-each>
                    </xsl:if>

                    <!-- CR & Tab -->
                    <xsl:text>&#x0A;</xsl:text>
                    <xsl:text>&#x09;</xsl:text>
                    <xsl:text>&#x09;</xsl:text>
                </xsl:if>
            </xsl:for-each>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
