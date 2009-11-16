<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
	version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru"	>

  <xsl:output
    name="standardHtmlOutputFormat"
    method="html"
    indent="yes"
    omit-xml-declaration="yes"
    doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
      doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
    />

  <xsl:template match="/">
    <h1>
      Release Notes
    </h1>
    <xsl:for-each select="/rss/channel/item">
      <h3>
        <a href="#{key}">
          <xsl:value-of select="title"/>
        </a>
      </h3>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>