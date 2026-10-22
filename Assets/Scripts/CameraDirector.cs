using UnityEngine;

/// <summary>
/// Dirige la cámara según las colisiones importantes de la máquina.
/// </summary>
public class CameraDirector : MonoBehaviour
{
    [Header("Encuadre")]
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public Vector3 offsetPlanoAmplio = new Vector3(0f, 7f, -12f);
    public Vector3 offsetPlanoFinal = new Vector3(0f, 18f, -28f);
    public float velocidadDeSuavizado = 5f;

    private Transform objetivoActual;
    private Vector3 offsetActual;
    private Etapa etapa;
    private bool enfocarFichas12A16;

    private enum Etapa
    {
        SiguiendoStarterSphere,
        SiguiendoEsfera2,
        SiguiendoEsfera3,
        SiguiendoBalon,
        SiguiendoCylinder,
        MostrandoFichas17A9,
        SiguiendoBalon1,
        MostrandoFichas12A16,
        SiguiendoEsfera2Dos,
        SiguiendoBalon2,
        MostrandoSistemaCompleto
    }

    private void Awake()
    {
        offsetActual = offset;
        objetivoActual = Buscar("StarterSphere");

        RegistrarRelay("StarterSphere");
        RegistrarRelay("esfera2");
        RegistrarRelay("esfera3");
        RegistrarRelay("balon");
        RegistrarRelay("Cylinder (3)");
        RegistrarRelay("ficha (9)");
        RegistrarRelay("balon (1)");
        RegistrarRelay("ficha (11)");
        RegistrarRelay("ficha (16)");
        RegistrarRelay("esfera2 (2)");
        RegistrarRelay("balon (2)");
    }

    private void LateUpdate()
    {
        Vector3 puntoDeInteres;
        if (enfocarFichas12A16)
        {
            puntoDeInteres = ObtenerCentroFichas12A16();
        }
        else if (objetivoActual != null)
        {
            puntoDeInteres = objetivoActual.position;
        }
        else
        {
            return;
        }

        float suavizado = Mathf.Clamp01(
            Mathf.Max(0f, velocidadDeSuavizado) * Time.deltaTime);
        Vector3 posicionDeseada = puntoDeInteres + offsetActual;

        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado);

        Vector3 direccion = puntoDeInteres - transform.position;
        if (direccion.sqrMagnitude > 0.001f)
        {
            transform.LookAt(puntoDeInteres);
        }
    }

    private void ProcesarColision(string nombreOrigen, Transform objetoDestino)
    {
        switch (etapa)
        {
            case Etapa.SiguiendoStarterSphere:
                if (EsColision(nombreOrigen, "StarterSphere", objetoDestino, "esfera2"))
                {
                    CambiarObjetivo("esfera2", Etapa.SiguiendoEsfera2, offset);
                }
                break;

            case Etapa.SiguiendoEsfera2:
                if (EsColision(nombreOrigen, "esfera2", objetoDestino, "esfera3"))
                {
                    CambiarObjetivo("esfera3", Etapa.SiguiendoEsfera3, offset);
                }
                break;

            case Etapa.SiguiendoEsfera3:
                if (EsColision(nombreOrigen, "esfera3", objetoDestino, "balon"))
                {
                    CambiarObjetivo("balon", Etapa.SiguiendoBalon, offset);
                }
                break;

            case Etapa.SiguiendoBalon:
                if (EsColision(nombreOrigen, "balon", objetoDestino, "soporte 2 (1)"))
                {
                    CambiarObjetivo("Cylinder (3)",
                        Etapa.SiguiendoCylinder,
                        offset);
                }
                break;

            case Etapa.SiguiendoCylinder:
                if (EsColision(nombreOrigen, "Cylinder (3)", objetoDestino, "ficha (17)"))
                {
                    offsetActual = offsetPlanoAmplio;
                    etapa = Etapa.MostrandoFichas17A9;
                }
                break;

            case Etapa.MostrandoFichas17A9:
                if (EsColision(nombreOrigen, "ficha (9)", objetoDestino, "balon (1)"))
                {
                    CambiarObjetivo("balon (1)", Etapa.SiguiendoBalon1, offset);
                }
                break;

            case Etapa.SiguiendoBalon1:
                if (EsColision(nombreOrigen, "balon (1)", objetoDestino, "ficha (11)"))
                {
                    objetivoActual = null;
                    offsetActual = offsetPlanoAmplio;
                    enfocarFichas12A16 = true;
                    etapa = Etapa.MostrandoFichas12A16;
                }
                break;

            case Etapa.MostrandoFichas12A16:
                if (EsColision(nombreOrigen, "ficha (16)", objetoDestino, "esfera2 (2)"))
                {
                    CambiarObjetivo("esfera2 (2)",
                        Etapa.SiguiendoEsfera2Dos,
                        offset);
                }
                break;

            case Etapa.SiguiendoEsfera2Dos:
                if (EsColision(nombreOrigen, "esfera2 (2)", objetoDestino, "balon (2)"))
                {
                    CambiarObjetivo("balon (2)", Etapa.SiguiendoBalon2, offset);
                }
                break;

            case Etapa.SiguiendoBalon2:
                if (EsColision(nombreOrigen, "balon (2)", objetoDestino, "model"))
                {
                    CambiarObjetivo("model",
                        Etapa.MostrandoSistemaCompleto,
                        offsetPlanoFinal);
                }
                break;
        }
    }

    private void CambiarObjetivo(string nombre, Etapa nuevaEtapa, Vector3 nuevoOffset)
    {
        Transform nuevoObjetivo = Buscar(nombre);
        if (nuevoObjetivo == null)
        {
            return;
        }

        objetivoActual = nuevoObjetivo;
        offsetActual = nuevoOffset;
        enfocarFichas12A16 = false;
        etapa = nuevaEtapa;
    }

    private Vector3 ObtenerCentroFichas12A16()
    {
        Vector3 centro = Vector3.zero;
        int cantidad = 0;

        for (int numero = 12; numero <= 16; numero++)
        {
            Transform ficha = Buscar("ficha (" + numero + ")");
            if (ficha == null)
            {
                continue;
            }

            centro += ficha.position;
            cantidad++;
        }

        if (cantidad == 0)
        {
            return objetivoActual != null ? objetivoActual.position : transform.position;
        }

        return centro / cantidad;
    }

    private bool EsColision(
        string origen,
        string origenEsperado,
        Transform destino,
        string destinoEsperado)
    {
        return EsNombre(origen, origenEsperado) &&
            EsNombre(destino, destinoEsperado);
    }

    private bool EsNombre(string nombreReal, string nombreEsperado)
    {
        return nombreReal == nombreEsperado;
    }

    private bool EsNombre(Transform objeto, string nombreEsperado)
    {
        Transform actual = objeto;
        while (actual != null)
        {
            if (actual.name == nombreEsperado)
            {
                return true;
            }

            actual = actual.parent;
        }

        return false;
    }

    private Transform Buscar(string nombre)
    {
        GameObject objeto = GameObject.Find(nombre);
        return objeto == null ? null : objeto.transform;
    }

    private void RegistrarRelay(string nombre)
    {
        Transform objeto = Buscar(nombre);
        if (objeto == null)
        {
            return;
        }

        Transform[] jerarquia = objeto.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < jerarquia.Length; i++)
        {
            CollisionRelay relay = jerarquia[i].GetComponent<CollisionRelay>();
            if (relay == null)
            {
                relay = jerarquia[i].gameObject.AddComponent<CollisionRelay>();
            }

            relay.Configurar(this, nombre);
        }
    }

    private sealed class CollisionRelay : MonoBehaviour
    {
        private CameraDirector director;
        private string nombreDelObjeto;

        public void Configurar(CameraDirector cameraDirector, string nombre)
        {
            director = cameraDirector;
            nombreDelObjeto = nombre;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (director != null)
            {
                director.ProcesarColision(nombreDelObjeto, collision.transform);
            }
        }
    }

}
