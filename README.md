# RULER OF THE KINGDOM

Il gioco è basato sulla gestione del regno
L'obiettivo è quello di mantenere il proprio regno e allo stesso tempo di espandersi conquistando altri regni.
Bisogna sempre tenere a mente che bisogna trovare un equilibrio tra tasso di felicità della popolazione e conquiste per non perdere il regno e subire un colpo di stato.


# DEFINIZIONE DEI REQUISITI E ANALISI

L'applicazione dovrà consentire al giocatore di espandersi e acquistare i vari regni senza perdere il proprio.
Le battaglie si risolveranno tramite il lancio dei dadi e termineranno con la perdita o la conquista delle varie regioni.
Se il giocatore perderà tutte le regioni perderà la partita.
Il giocatore dovrà fare attenzione a tutti i parametri del regno dal budget,ai territori al tasso di felicità della popolazione.
L'applicazione gestirà tali parametri e li memorizzerà anche tramite la persistenza dei dati.
Se il giocatore dovesse conquistare tutti i regni vincerà la partita.




# PIANIFICAZIONE E DESIGN DELL'ARCHITETTURA

- [ ] Creazione di un menu generale con opzioni di scelta

- [ ] Creazione del proprio regno sarà un oggetto generico

- [ ] Le caratteristiche dell'oggetto saranno: nome regno,tipo di regno(Principato,repubblica),budget di default di 1000000 da destinare a guerre o al popolo,tasso di soddifazione del popolo da 0 a 100,tasse,esercito,nome delle 3 regioni del regno.

- [ ] Il tipo di regno scelto se è un principato aumenterà il budget a disposizione ma il valore di default del tasso di felicità sarà più basso viceversa se si sceglie una repubblica il tasso di felicità aument

- [ ] Ogni regno è diviso in tre regioni.Se tutte e tre le regioni vengono sconfitte hai acquisito il regno avversario

- [ ] Esistono in tutto 4 regni compreso quello creato dall'utente

- [ ] dinamica lancio dadi.Il primo giocatore tira 2 dadi  al turno successivo il secondo giocatore ovvero il pc lancerà 2 dadi.Dopo 2 giocate chi vince acquisterà la regione avversaria con un incremento del budget e del tasso di felicità.Se si conquisteranno tutte e 3 le regioni si otterrà il regno con un incremento ancora superiore del budget e del tasso di felicità della popolazione  

- [ ] Sistema di punteggio del regno per ogni regione conquistata se conquisti tutto il regno punti extra 


# PRIMA VERSIONE SEMPLIFICATA

- [ ] Creazione di un menu con le opzioni di scelta

- [ ] Opzione gioca: chiede di creare il proprio regno con le caratteristiche previste.Verrà creato un file json con le caratteristiche del regno messe dall' utente

- [ ] Le caratteristiche dell'oggetto saranno: nome regno,tipo di regno(Principato,repubblica),budget da destinare a guerre o al popolo,tasso di soddifazione del popolo da 0 a 100,tasse,esercito,nome delle 3 regioni del regno.

- [ ] Opzione esci dal gioco

- [ ] Creazione di un regno di default da sfidare 

- [ ] Lancio di 2 dadi ciascuno.Ogni 2 vittorie si acquista un regno


# SECONDA VERSIONE 

- [ ] Creazione di altri 2 regni di default da sfidare 

- [ ] OpzioneMenu di scelta del regno da sfidare 

- [ ] Funzione di visualizzazione delle caratteristiche del proprio regno

- [ ]  Implementazione delle regioni

- [ ]  possibilità di scegliere quale regione attaccare con un sottomenu per scegliere quale regione sfidare

- [ ]  Implementazione della logica dei dadi applicata alle singole regioni invece che al regno

- [ ] vengono sottratti fondi pubblici e aumentate le tasse tutte le volte che si fa guerra riducendo il tasso di felicità della popolazione

- [ ] se una regione viene conquistata si recuperano le spese e aumenta il budget del 10%


# TERZA VERSIONE

- [ ] Implementazione di una logica politica se il tasso di felicità da 0 a 100 scende sotto il 20% ci sarà una rivolta e verranno perse 2 regioni

- [ ] valutare se implementare un advisor  per ricevere dei suggerimenti sulla guerra o sulla gestione del regno

- [ ] Riducendo le tasse, oppure vincendo delle regioni aumenterà il tasso di felicità della popolazione

- [ ] se tutti i regni vengono conquistati la partita finisce con un determinato score .

- [ ] se si perde rimane lo score

- [ ] lo score verrà registrato in un file


# QUARTA VERSIONE IMPLEMENTAZIONE DI SPECTRE 

- [ ] Implementare spectre per il menu e sottomenu

- [ ] creazione di tabelle per descrivere vincite e dati 








