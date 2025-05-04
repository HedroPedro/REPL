# REPL

## Gramática

S -> ID | E
ID -> LA
L -> var (\[A-Za-z\]+)
A -> = E | $\epsilon$
E -> VR
R -> +E | -E | F
F -> *E | /E | $\epsilon$
V -> L | N | (E)
N -> num \[0-9\]+
