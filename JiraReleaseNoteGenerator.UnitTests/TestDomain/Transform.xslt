<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
	version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ru="http://www.rubicon-it.com"
  xmlns:functx="http://www.functx.com"
	exclude-result-prefixes="xs fn ru functx"
	>

  <xsl:template match="/">
    <xsl:call-template name="htmlSite">
      <xsl:with-param name="siteTitle" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="htmlSite">
    <xsl:param name="siteTitle" />

    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>
          <xsl:value-of select="$siteTitle" />
        </title>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
      </head>
      <body>
        <h1>
          Release Notes
        </h1>
        <xsl:for-each select="//channel/item">
          <xsl:value-of select="title"/>
        </xsl:for-each>
      </body>
    </html>

  </xsl:template>

</xsl:stylesheet>