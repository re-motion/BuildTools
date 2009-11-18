<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
	version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru"
	>

  <xsl:output
	name="standardHtmlOutputFormat"
	method="html"
	indent="yes"
	omit-xml-declaration="yes"
	doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
    />

  <xsl:template match="/">
    <xsl:call-template name="htmlSite">
      <xsl:with-param name="siteTitle" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="htmlSite">
    <xsl:param name="siteTitle" />
    <xsl:result-document format="standardHtmlOutputFormat">
      <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
          <title>
            <xsl:value-of select="$siteTitle" />
          </title>
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!-- include resources -->
          <link rel="stylesheet" type="text/css" href="style.css" />
        </head>
        <body>
          <h1>
            Release Notes
          </h1>

          <xsl:for-each select="/rss/issueOrder/issue">
            <h3>
              <xsl:value-of select="title"/>
            </h3>
            <xsl:variable name="selectingType" select="type"/>
            <xsl:call-template name="issueListForType">
              <xsl:with-param name="root" select="/" />
              <xsl:with-param name="issues" select="/rss/channel/item[type=$selectingType]"/>
            </xsl:call-template>
          </xsl:for-each>
        </body>
      </html>
    </xsl:result-document>
  </xsl:template>

  <xsl:template name="issueListForType">
    <xsl:param name="root" />
    <xsl:param name="issues" />

    <xsl:if test="count($issues) = 0">
      <div class="listEntry">(none)</div>
    </xsl:if>

    <xsl:for-each select="$issues">
      <div class="listEntry">

        <xsl:if test="status = 'Closed'">
          <a href="#{key}">
            <xsl:value-of select="title"/>
          </a>
        </xsl:if>
        <xsl:if test="status != 'Closed'">
          <span class="notClosedIssue">
            <a href="#{key}">
              <xsl:value-of select="title"/>
            </a>
          </span>
        </xsl:if>

      </div>

      <xsl:call-template name="listChildren">
        <xsl:with-param name="root" select="$root" />
        <xsl:with-param name="key" select="key" />
      </xsl:call-template>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="listChildren">
    <xsl:param name="root"/>
    <xsl:param name="key"/>

    <xsl:for-each select="$root//rss/channel/item[parent = $key]">
      <div class="children">
        <a href="#{key}">
          <xsl:value-of select="title"/>
        </a>
      </div>
    </xsl:for-each>

  </xsl:template>

</xsl:stylesheet>