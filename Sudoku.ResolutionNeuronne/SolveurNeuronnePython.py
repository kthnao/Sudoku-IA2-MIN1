import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
from timeit import default_timer
import copy


def preprocess_sudoku(grid):
    """
    Prépare une grille de Sudoku donnée sous forme de liste 2D pour le modèle.
    """
    # Mettre en forme le Sudoku
    quiz_array = np.array(grid).reshape(9, 9, 1)
    quiz_array = quiz_array / 9 - 0.5  # Normalisation
    return quiz_array

def predict_sudoku(model, quiz_array):
    """
    Prédit la solution d'un Sudoku avec le modèle.
    """
    # Ajouter une dimension batch pour le modèle
    quiz_array = np.expand_dims(quiz_array, axis=0)
    
    # Prédiction
    predictions = model.predict(quiz_array)
    
    # Décoder les prédictions
    predicted_sudoku = np.argmax(predictions, axis=-1).reshape(9, 9) + 1
    
    return predicted_sudoku

def is_valid_sudoku(sudoku):
    """
    Vérifie si un Sudoku est valide.
    """
    def is_valid_block(block):
        return sorted(block) == list(range(1, 10))
    
    # Vérification des lignes
    for row in sudoku:
        if not is_valid_block(list(row)):
            return False
    
    # Vérification des colonnes
    for col in sudoku.T:
        if not is_valid_block(list(col)):
            return False
    
    # Vérification des sous-grilles 3x3
    for i in range(0, 9, 3):
        for j in range(0, 9, 3):
            block = sudoku[i:i+3, j:j+3].flatten()
            if not is_valid_block(list(block)):
                return False
    
    return True

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
model = load_model('C:/Users/etien/Downloads/sudoku_solver_final.h5')

start = default_timer()
# Préparer l'entrée pour le modèle
quiz_array = preprocess_sudoku(instance)

# Prédire la solution
resultt = predict_sudoku(model, quiz_array)

# Convertir en tableau int[,]
result = np.array(resultt, dtype=np.int32)


# Vérification de validité
if is_valid_sudoku(resultt):
    print("Le Sudoku prédit est valide.")
else:
    print("Le Sudoku prédit n'est pas valide.")


if all(all(cell != 0 for cell in row) for row in result):
    print("Sudoku résolu par le modèle CNN :")
    for row in result:
        print(row)
else:
    print("Aucune solution trouvée.")
execution = default_timer() - start
print("Le temps de résolution est de : ", execution * 1000, " ms")