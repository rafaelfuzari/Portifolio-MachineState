# Portfolio-StateMachine

English

State Machine system developed in Unity, with the states: idle, walking, alert, firing, chasing, checking and dead.  
This project was made to show skills in C# and IA programming.

Features:   
-Enemy will patrol an area, which is decided by waypoints;   
-If he sees a player will go to a alert state;   
-If the player remain in the vision area for 1 second, the enemy will start shooting him; 
-If the player escape the area, the enemy will chase him;  
-If the player runaway, the enemy will return to patrol;  
-If during the chase the player hide behind some obstacle, the enemy will go to player's last position and check the area searching for him, if he did not find anything, will return to patrol;  

The enemy vision area is small, and will only detect the player if he is front of him.
    
All the codes are in Assets>Scripts.  

Playable version: https://lazylionstudio.itch.io/portfolio-state-machine  

-------------

Português

Sistema de estado de máquina feito no Unity, com os estados: ocioso, andando, alerta, atirando, perseguindo, checando e morto.  
Esse projeto foi feito para demonstrar habilidades de programação em C# e IA.  

Características:  
-O inimigo vai patrulhar uma área, que é decidida por waypoints;  
-Se ele ver o player, vai para o estado de alerta;  
-Se o player ficar na area de visão por um segundo, o inimigo vai começar a atirar nele;  
-Se o player escapar da área, o inimigo vai perseguir ele;  
-Se o player fugir, o inimigo vai voltar a patrulhar;  
-Se durante a perseguição o player se esconder atrás de um obstáculo, o inimigo vai para a última posição do player e checar a área procurando por ele, se não achar nada, vai voltar a patrulhar;  

A área de visão do inimigo é pequena, e só vai detectar o player se ele estiver na sua frente.  

Todos os códigos estão em Assets>Scripts.   

Versão jogável: https://lazylionstudio.itch.io/portfolio-state-machine   
