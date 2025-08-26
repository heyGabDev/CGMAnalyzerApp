## Architecture des lecteurs CGM

- `StreamArgumentReader.cs` lit les arguments directement dans un fichier .cgm à l'aide de `BinaryReader`.
- `ExtractedArgumentReader.cs` lit les arguments dans un tableau d’octets extrait précédemment.

Chaque `CgmCommand` peut donc être construit :
- Soit avec une lecture différée (en mémoire)
- Soit avec une lecture directe (depuis fichier)

Cela permet de mieux séparer les responsabilités : lecture binaire et traitement logique.
