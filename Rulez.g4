parser grammar Rulez;
options {
	tokenVocab=RulezBase;
	}
tokens {}

expression:
		simple_expression (expr_op simple_expression)*	
		;

expr_op:
		AND | XOR | OR | NOT
		;

simple_expression:
		left_element relational_op right_element
		;

relational_op:
		EQ | LTH | GTH | NOT_EQ | LET | GET  ;

element:
		USER_VAR | ID | ('|' ID '|') | INT
	;

right_element:
		element
	;

left_element:
		element
		;
		