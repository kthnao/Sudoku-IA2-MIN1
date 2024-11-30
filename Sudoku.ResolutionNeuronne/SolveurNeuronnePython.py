import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from timeit import default_timer
import copy


# Fonction de normalisation (mise à l'échelle des valeurs entre -0.5 et 0.5)
def norm(grid):
    return (grid / 9.0) - 0.5

# Fonction de dénormalisation (inverse de la normalisation)
def denorm(grid):
    return (grid + 0.5) * 9.0

# Fonction pour résoudre le Sudoku avec le modèle CNN
def inference_sudoku(sample):
    feat = copy.copy(sample)

    while(1):
        # Prediction des valeurs
        out = model.predict(feat.reshape((1, 9, 9, 1)))
        out = out.squeeze()

        # Récupérer les valeurs prédites
        pred = np.argmax(out, axis=1).reshape((9, 9)) + 1

        # Récupérer les probabilités pour chaque valeur
        prob = np.around(np.max(out, axis=1).reshape((9, 9)), 2)

        # Créer un masque pour les cases vides
        feat = denorm(feat).reshape((9, 9))
        mask = (feat == 0)

        # Si aucune case vide n'est trouvée, sortir de la boucle
        if(mask.sum() == 0):
            break

        # Récupérer les probabilités pour les cases vides
        prob_new = prob * mask

        # Trouver l'indice de la plus haute probabilité
        ind = np.argmax(prob_new)

        # Trouver la ligne et la colonne correspondantes
        x, y = (ind // 9), (ind % 9)

        # Récupérer la valeur prédite pour cette case
        val = pred[x][y]

        # Assigner cette valeur à la case
        feat[x][y] = val

        # Repasser la grille avec la valeur ajoutée au modèle pour obtenir la suivante
        feat = norm(feat)

    return pred

# Définir instance uniquement si non déjà défini par PythonNET
if 'instance' not in locals():
    instance = np.array([
        [0,0,0,0,9,4,0,3,0],
        [0,0,0,5,1,0,0,0,7],
        [0,8,9,0,0,0,0,4,0],
        [0,0,0,0,0,0,2,0,8],
        [0,6,0,2,0,1,0,5,0],
        [1,0,2,0,0,0,0,0,0],
        [0,7,0,0,0,0,5,2,0],
        [9,0,0,0,6,5,0,0,0],
        [0,4,0,9,7,0,0,0,0]
    ], dtype=int)

# Charger le modèle CNN pré-entrainé
model = load_model('C:/Users/etien/Downloads/sudoku_model_09605_10.h5')

start = default_timer()
# Résoudre le Sudoku avec CNN
# Normaliser la grille avant de la passer au modèle
grid_preprocessed = norm(instance)

# Résoudre le Sudoku
resultt = inference_sudoku(grid_preprocessed)

print(resultt)

# Convertir en tableau int[,]
result = np.array(resultt, dtype=np.int32)



if all(all(cell != 0 for cell in row) for row in result):
    print("Sudoku résolu par le modèle CNN :")
    for row in result:
        print(row)
else:
    print("Aucune solution trouvée.")
execution = default_timer() - start
print("Le temps de résolution est de : ", execution * 1000, " ms")
