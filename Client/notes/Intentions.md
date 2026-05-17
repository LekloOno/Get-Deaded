# Get Deaded

Le projet vient d'une volonté de mêler ce qui, aujourd'hui, me plaît dans un FPS.
En particulier, dans les FPS Solo existants aujourd'hui, aucun ne présente de réel challenge en terme de visée, et ils se focalisent de manière générale peu sur les fondamentaux de la gestion "micro" dans un FPS. Le but est aussi de construire un produit qui le permet.
Un environnement Solo dans lequel le joueur doit user de techniques similaires à ce qu'il ferait dans un FPS multijoueur compétitif.

C'est à dire -
    - Des mouvements au service de son contrôle de l'espace
    - Des mouvements de combat - au service de sa visée ou de sa survivabilité
    - La gestion micro de son positionnement - rupture, optimisation d'angle d'exposition, prévision d'escape route, etc.
    - La gestion micro de son combat - "Bait" l'assaut d'un adversaire pour s'offrir une fenêtre de manoeuvre libre, tempo, etc.
    - Une visée exigente, mêlant toutes les composantes - réactivité, lecture de mouvements, smoothness, timing et snappiness. 

## Des mécaniques de mouvements techniques et complexes
> Offrir une pallette d'expression mécanique large, profonde et exigeante, au travers des mouvements.  

Il doit toujours être possible d'être meilleur pour se déplacer. Plus efficacement, plus rapidement, avec plus de contrôle, être plus imprédictible, ou plus évasif, etc..  

Offrir un terrain de jeu mécanique qui permet au débutant de se déplacer sans encombres, et au passionné de développer et perfectionner ses talents sur des centaines et milliers d'heures. Que ce soit au travers d'input ou de timing précis, de combinaisons de mécaniques, d'optimisations..

Dans la direction globale des mouvements, il surtout s'agit offrir au joueur expérimenté du contrôle plutôt que de la vitesse.  

Une comparaison analogue serait celle entre Quake et Apex. Maîtriser les techniques de mouvement avancées de Quake permet surtout de gagner significativement en vitesse, là ou dans Apex, le gain de vitesse général est plus accessoire. Il s'agit surtout de savoir accélérer plus vite, de savoir modifier sa trajectoire pour se placer où souhaité, et d'être plus imprédictible, plus évasif. La balance visée se situe entre les deux, tout de même plus proche d'Apex. Une emphase principale sur la capacité de contrôle.  
</br>

Au delà des déplacements, il s'agit aussi des *mouvements de combat*. Dans un FPS multijoueur, les joueurs doivent, au delà d'apprendre à se déplacer, apprendre aussi à rendre leurs mouvements plus imprédictibles et/ou complexes. Ainsi ils augmentent leur survivabilité en combat, mais doivent apprendre à le faire tout en limitant l'impact que cela à sur leur propre visée. Il s'agira alors de développer des IAs d'ennemis qui émulent plus ou moins la façon qu'a un humain de viser.
</br>

*ref - Apex, titanfall, team fortress, rocket league ..*


## Une aim demandante
> Permettre l'expression de l'ensemble des talents de visée du joueur. Que celle-ci soit aussi importante que ses mouvements.  

Aujourd'hui, les FPS solo en manquent cruellement, probablement par simple choix, ou par défi technique.  D'autre part, les fps multijoueurs eux, permettent cette complexité au travers de facteurs parfois très différents.  

### Complexité de mouvements
D'un côté, certains FPS compétitifs apportent de l'exigence de visée par des mouvements rapides, erratiques, évasifs, et très variés.
> Exemple - Apex, Overwatch, Quake, Team Fortress ..  

### Précision et rapidité
De l'autre, certains abordent une approche plus précise, et des timings plus courts.  
> Exemple - Counter Strike, Valorant, ..

### Snappiness
Certains encore mêlent timings courts et mouvements complexes, menant à un type d'aim plus *"snappy"*.
> Exemple - Cod, Battlefield, ..

### Imprédictibilité
Enfin, tous ces jeux ajoutent à ces caractéristiques quelque chose de propre au fait de jouer contre d'autres humains : l'imprédictibilité, la variété.

Elle est particulièrement forte dans les jeux qui offrent des options de mouvements erratiques (typiquement, overwatch).

## Solo

Les FPS solo eux, visent en général plus la connaissance de jeu, et éventuellement, la maîtrise des mécaniques de mouvements.

L'aim s'y limite généralement à ***bien regarder*** plutôt que ***bien viser***, avec des cibles relativement grosses, aux mouvements peu complexes, souvent aussi peu rapides.  


### Objectif
On veut construire un environnement de jeu Solo dans lequel on pourrait retrouver l'ensemble de ces contextes d'aim (avec une emphase sur l'aspect *complexité* de mouvement).

Que le joueur qui maîtrise particulièrement sa *lecture de mouvements*, sa *réactivité*, sa *smoothness*, sa *snappiness*, son *dynamic clicking*, son *leading*, soit réellement récompensé.  
Qu'il puisse en faire une **différence majeure**.

Qu'il puisse à la fois décider de se spécialiser dans l'un, en étant suffisamment stratège, ou bien d'être polyvalent.

On pourra s'inspirer des scénarios d'Aim Lab/Kovaaks, étudier les mouvements de joueurs en jeu, profiter de l'environnement solo pour offrir des ennemis et scénarios divers et trouver le juste équilibre.

## Une gestion micro

Il existe encore un grand nombres d'éléments pouvant être associées aux "mécaniques". Des éléments de micro gestion, d'optimisation, tel que la gestion des angles d'exposition et leur découpe, la temporisation et la manipulation du comportement de son adversaire. Ce sont l'ensemble de ces éléments abordés dans les points précédents, et de ces éléments de micro gestions, qui doivent être sujet à une attention de GD particulière.

La connaissance de jeu "volumétrique" (assimiler de grandes quantités d'informations spécifiques, typiquement ce que l'on peut retrouver de manière prédominante dans LoL) et la macro-gestion ne doivent pas effacer celles-ci. A l'inverse de ce que l'on retrouve dans la majorité des shooters solo comme multijoueurs, ou une micro-gestion et mécaniques parfaites ne sont souvent que très peu utile sans une macro-gestion solide.

La connaissance de jeu doit se limiter au maximum à des concepts plutôt qu'à des données. Des mécaniques générales de jeu, qui peuvent être rapidement assimilées, puis perfectionnées, ou dans le plus permissif des cas, une connaissance certes spécifique et nouvelle mais qui ne soit pas "game changer".

## Un environnement compétitif Solo

Dans tout cela, il s'agit aussi d'offrir un environnement compétitif fort, malgré la composante solo. Encourager le joueur à recommencer les missions, à les perfectionner. Mettre l'emphase sur l'aspect compétitif.

J'aime particulièrement l'aspect compétitif dans des environnements solo. Je le trouve particulièrement sain, mais je le trouve aussi très addictif.

Il ne s'agit plus d'une confrontation directe à un adversaire. Il s'agit de repousser ses propres limites, de se battre soi-même.  
On dispose du plein contrôle de notre évolution, pas de coéquipiers et adversaires aléatoires, d'armes ou de héros que l'on estime trop fort. On est le plein responsable de nos victoires comme échecs, ce qui offre une progression plus prédictible, plus régulière.  

Quelque chose que j'ai pu retrouver en m'amusant à *grind* les leaderboards d'aimlab pendant de très nombreuses heures de jeu, mais aussi dans d'autres activités comme l'escalade, la callisthénie, le jonglage ou quelques acrobaties à vélo.  

J'aime aussi l'idée du theorycraft. Offrir au joueur la possibilité d'ajuster son gameplay avec des équipements et capacités. Ces instants de réflexions sont précieux.  
Peut-être plus tard développer un système de saison, avec des contraintes sur ce même theorycraft. Un map pool, des bans d'équipements, ..


## Un terrain d'expérimentation

Profiter de cet environnement pour expérimenter avec diverses mécaniques de jeu. Changer le rythme en poussant à l'exploration des maps à la recherche de secrets, de ressources. Expérimenter avec la rejouabilité, la progression, etc.  

Proposer des modes de jeu complètement différents pour explorer d'autres rythmes, des défis spécifiques à certaines mécaniques, etc.