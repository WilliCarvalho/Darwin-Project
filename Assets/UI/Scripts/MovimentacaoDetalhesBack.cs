using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentacaoDetalhesBack : MonoBehaviour
{
    public float velocidade;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoverParaCima());
    }

    private void Update()
    {
        if (transform.position.y > 2167f)
        {
            transform.position = transform.position + new Vector3(transform.position.y, -8f, transform.position.z);
            print("io");
        }
    }

    IEnumerator MoverParaCima()
    {
       
        transform.position += Vector3.up * velocidade * Time.deltaTime;
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(MoverParaCima());
    }
}