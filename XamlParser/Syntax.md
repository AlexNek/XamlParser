## Declaring Namespaces

[1]   	NSAttName	   ::=   	PrefixedAttName | DefaultAttName

[2]   	PrefixedAttName	   ::=   	'xmlns:' NCName	[NSC: Reserved Prefixes and Namespace Names]

[3]   	DefaultAttName	   ::=   	'xmlns'

[4]   	NCName	   ::=   	Name - (Char* ':' Char*)	/* An XML Name, minus the ":" */

```
<x xmlns:edi='http://ecommerce.example.org/schema'>
  <!-- the "edi" prefix is bound to http://ecommerce.example.org/schema
       for the "x" element and contents -->
</x>
```
## Qualified Name

[7]   	QName	   ::=   	PrefixedName | UnprefixedName

[8]   	PrefixedName	   ::=   	Prefix ':' LocalPart

[9]   	UnprefixedName	   ::=   	LocalPart

[10]   	Prefix	   ::=   	NCName

[11]   	LocalPart	   ::=   	NCName

## Element Names

[12]   	STag	   ::=   	'<' QName (S Attribute)* S? '>' 	[NSC: Prefix Declared]

[13]   	ETag	   ::=   	'</' QName S? '>'	[NSC: Prefix Declared]

[14]   	EmptyElemTag	   ::=   	'<' QName (S Attribute)* S? '/>'	[NSC: Prefix Declared]

```
<!-- the 'price' element's namespace is http://ecommerce.example.org/schema -->
  <edi:price xmlns:edi='http://ecommerce.example.org/schema' units='Euro'>32.18</edi:price>
```

## Attribute

[15]   	Attribute	   ::=   	NSAttName Eq AttValue
			| QName Eq AttValue	[NSC: Prefix Declared]
				[NSC: No Prefix Undeclaring]
				[NSC: Attributes Unique]

```
<x xmlns:edi='http://ecommerce.example.org/schema'>
  <!-- the 'taxClass' attribute's namespace is http://ecommerce.example.org/schema -->
  <lineItem edi:taxClass="exempt">Baby food</lineItem>
</x>
```

## Qualified Names in Declarations
[16]   	doctypedecl	   ::=   	'<!DOCTYPE' S QName (S ExternalID)? S? ('[' (markupdecl | PEReference | S)* ']' S?)? '>'

[17]   	elementdecl	   ::=   	'<!ELEMENT' S QName S contentspec S? '>'

[18]   	cp	   ::=   	(QName | choice | seq) ('?' | '*' | '+')?

[19]   	Mixed	   ::=   	'(' S? '#PCDATA' (S? '|' S? QName)* S? ')*'
			| '(' S? '#PCDATA' S? ')'

[20]   	AttlistDecl	   ::=   	'<!ATTLIST' S QName AttDef* S? '>'

[21]   	AttDef	   ::=   	S (QName | NSAttName) S AttType S DefaultDecl
