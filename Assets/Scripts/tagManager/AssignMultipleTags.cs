using UnityEngine;
using System.Collections.Generic;

public class AssignMultipleTags : MonoBehaviour
{
    [SerializeField] private List<string> tags = new List<string>(); // Usamos List para el Inspector

    // Verifica si tiene un tag
    public bool HasTag(string tagToCheck) => tags.Contains(tagToCheck);

    // Añade un tag (con validación)
    public void AddTag(string newTag)
    {
        if (string.IsNullOrEmpty(newTag))
        {
            Debug.LogWarning("El tag no puede estar vacío", this);
            return;
        }
        if (!tags.Contains(newTag))
        {
            tags.Add(newTag);
            Debug.Log($"Tag añadido: '{newTag}' en {gameObject.name}", this);
        }
    }

    // Elimina un tag
    public void RemoveTag(string tagToRemove)
    {
        if (tags.Remove(tagToRemove))
        {
            Debug.Log($"Tag eliminado: '{tagToRemove}' de {gameObject.name}", this);
        }
    }

    // Imprime TODOS los tags en la consola
    public void LogCurrentTags()
    {
        Debug.Log($"Tags de {gameObject.name}: {string.Join(", ", tags)}", this);
    }

    // Opcional: Mostrar tags en el Update (para pruebas)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LogCurrentTags(); // Presiona 'T' para ver los tags
        }
    }
}