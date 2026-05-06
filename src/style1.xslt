<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="3.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="/Pay">
    <Employees>
      <xsl:for-each-group select="item" group-by="concat(@name, '|', @surname)">
        <Employee name="{@name}" surname="{@surname}">
          <xsl:for-each select="current-group()">
            <salary amount="{@amount}" mount="{@mount}"/>
          </xsl:for-each>
        </Employee>
      </xsl:for-each-group>  
    </Employees>
  </xsl:template>
</xsl:stylesheet>