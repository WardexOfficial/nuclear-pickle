# General messages
examine-nothing-worn = [color=gray]На н{ OBJECT($ent) } ничего не надето.[/color]
examine-nothing-worn-selfaware = [color=gray]На вас ничего не надето.[/color]

# Underwear slots
undershirt-examine = 
    • { CAPITALIZE(POSS-ADJ($ent)) } { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на { POSS-ADJ($ent) } теле.
underpants-examine = 
    • { CAPITALIZE(POSS-ADJ($ent)) } { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на { POSS-ADJ($ent) } теле.
socks-examine = 
    • { CAPITALIZE(POSS-ADJ($ent)) } { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на { POSS-ADJ($ent) } ногах.

# Selfaware underwear slots
undershirt-examine-selfaware = 
    • { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на вашем теле.
underpants-examine-selfaware = 
    • { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на вашем теле.
socks-examine-selfaware = 
    • { $id ->
        [empty] [bold]{ $item }[/bold]
       *[other] [enttex id="{ $id }" size={ $size }][bold]{ $item }[/bold]
    } на ваших ногах.

# Chest & groin exposure
examine-chest-groin-exposed = [color=red]{ CAPITALIZE(POSS-ADJ($ent)) } грудь и пах не прикрыты![/color]
examine-chest-groin-exposed-selfaware = [color=red]Ваши грудь и пах не прикрыты![/color]

examine-chest-exposed = [color=red]{ CAPITALIZE(POSS-ADJ($ent)) } грудь не прикрыта![/color]
examine-chest-exposed-selfaware = [color=red]Ваша грудь не прикрыта![/color]

examine-groin-exposed = [color=red]{ CAPITALIZE(POSS-ADJ($ent)) } пах не прикрыт![/color]
examine-groin-exposed-selfaware = [color=red]Ваш пах не прикрыт![/color]
