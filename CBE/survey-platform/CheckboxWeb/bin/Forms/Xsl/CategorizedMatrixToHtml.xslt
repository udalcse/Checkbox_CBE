<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="html" indent="yes"/>

    <xsl:include href="LabelledItemToHtml.xslt" />
    <xsl:include href="AnswerableItemToHtml.xslt" />

    <!-- Item template -->
    <xsl:template match="/item">
        <xsl:variable name="pkIndex" select="metaData/matrixMetaData/pkColumnIndex" />

        <div style="padding-top:5px;">
            <!-- Matrix Top-Level Question -->
            <xsl:apply-templates select="instanceData/texts" />

            <!-- Matrix-->
            <table class="matrix" cellspacing="0" cellpadding="2">
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
            </table>
        </div>
    </xsl:template>

    <!-- Row Template -->
    <xsl:template match="row">
        <xsl:param name="rowStyle" />
        <xsl:param name="pkIndex" />

        <xsl:variable name="rowType" select="@rowType "/>
        <xsl:variable name="rowText" select="text" />

        <!-- For first row, add an additional header row -->
        <xsl:if test="@rowNumber = '1'">
            <tr class="header">
                <xsl:for-each select="columns/column">
                    <xsl:if test="position() = $pkIndex">
                        <td class="Question"> </td>
                    </xsl:if>
                    <xsl:if test="position() != $pkIndex">
                        <td class="Question">
                            <xsl:value-of select="text" disable-output-escaping="yes" />
                        </td>
                    </xsl:if>
                </xsl:for-each>
            </tr>
        </xsl:if>

        <!-- Table with alternating style -->
        <tr>
            <!-- Add row style attribute -->
            <xsl:if test="$rowType = 'Subheading'">
                <xsl:attribute name="class">subheader</xsl:attribute>
                <td>
                    <xsl:attribute name="colspan"><xsl:value-of select="count(columns/column)"/></xsl:attribute>
                    <xsl:value-of select="$rowText" disable-output-escaping="yes"  />
                </td>
            </xsl:if>
            <xsl:if test="$rowType != 'Subheading'">
                <xsl:attribute name="class">
                    <xsl:value-of select="$rowStyle" />
                </xsl:attribute>

                <!-- Add columns, with special handling for pk column-->
                <xsl:for-each select="columns/column">
                    <!-- PK Column -->
                    <xsl:if test="position() = $pkIndex">
                        <td>
                            <xsl:if test="$rowType = 'Other'">
                                <xsl:value-of select="item/instanceData/answers/answer"/>
                            </xsl:if>
                            <xsl:if test="$rowType != 'Other'">
                                <xsl:value-of select="$rowText" disable-output-escaping="yes" />
                            </xsl:if>
                        </td>
                    </xsl:if>

                    <!-- Regular Column-->
                    <xsl:if test="position() != $pkIndex">
                        <td class="Answer" align="center">
                            <xsl:for-each select="item/instanceData/answers/answer">
                                <xsl:if test="position() > 1">, </xsl:if>
                                <xsl:value-of select="." disable-output-escaping="yes" />
                            </xsl:for-each>
                        </td>
                    </xsl:if>
                </xsl:for-each>
            </xsl:if>
        </tr>
    </xsl:template>
</xsl:stylesheet>
