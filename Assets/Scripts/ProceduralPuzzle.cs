using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class ProceduralPuzzle : MonoBehaviour
{
    [Header("Game Elements")]
    [SerializeField] private List<int> difficulties; 
    [SerializeField] private Transform gameHolder;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private List<AudioClip> PieceSFX;

    [Header("UI Elements")]
    [SerializeField] private List<Texture2D> imageTextures;
    [SerializeField] private Transform levelSelectPanel;
    [SerializeField] private Image levelSelectPrefab;
    [SerializeField] private GameObject starParticle;

    [Header("Sound Effects")]
    private SoundEffect soundEffect;
    private AudioSource sfxAudioSource;
    public AudioMixer audioMixer;
    public AudioClip puzzleSnap;
    public AudioClip puzzleScatter;
    public AudioClip puzzleDone;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;
    private Vector3 offset;
    private int piecesCorrect;

    private Transform draggingPiece = null;
    private GameObject hintQuad;

    private void Awake()
    {
        soundEffect = GetComponent<SoundEffect>();
        sfxAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        SpawnLevel();
    }

    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0; 

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit)
                    {
                        draggingPiece = hit.transform;
                        offset = draggingPiece.position - touchPosition;
                        offset += Vector3.back;
                    }
                    break;

                case TouchPhase.Moved:
                    if (draggingPiece)
                    {
                        Vector3 newPosition = touchPosition + offset;
                        draggingPiece.position = newPosition;
                    }
                    break;

                case TouchPhase.Ended:
                    if (draggingPiece)
                    {
                        SnapAndDisableIfCorrect();
                        draggingPiece.position += Vector3.forward;
                        draggingPiece = null;
                    }
                    break;
            }
        }
    }

    private void SpawnLevel()
    {

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameStateToLevelSelector();
        }

        for (int i = 0; i < imageTextures.Count; i++)
        {
            Texture2D texture = imageTextures[i];
            Image imagePrefab = Instantiate(levelSelectPrefab, levelSelectPanel);

            Transform imageTransform = imagePrefab.transform.Find("Image");

            if (imageTransform != null)
            {
                Image imageComponent = imageTransform.GetComponent<Image>();

                if (imageComponent != null)
                {
                    imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    Color initialColor = imageComponent.color;
                    imageComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

                    imageComponent.DOFade(1f, 0.5f).SetDelay(i * 0.75f);
                }
                else
                {
                    Debug.LogWarning("Image kosong");
                }
            }

            int levelIndex = i;
            imagePrefab.GetComponent<Button>().onClick.AddListener(delegate { StartGame(texture, levelIndex); });
        }
    }


    public void StartGame(Texture2D jigsawTexture, int levelIndex)
    {
        levelSelectPanel.gameObject.SetActive(false);

        pieces = new List<Transform>();

        int difficulty = difficulties[levelIndex];  

        dimensions = GetDimensions(jigsawTexture, difficulty);

        CreateJigsawPieces(jigsawTexture);

        Scatter();

        UpdateBorder(jigsawTexture);

        piecesCorrect = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameStateToPlaying();
        }
    }

    Vector2Int GetDimensions(Texture2D jigsawTexture, int difficulty)
    {
        Vector2Int dimensions = Vector2Int.zero;

        if (jigsawTexture.width < jigsawTexture.height)
        {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * jigsawTexture.height) / jigsawTexture.width;
        }
        else
        {
            dimensions.x = (difficulty * jigsawTexture.width) / jigsawTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    void CreateJigsawPieces(Texture2D jigsawTexture)
    {
        height = 1f / dimensions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameHolder);
                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;

                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = uv;
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    private void Scatter()
    {
        float orthoHeight = Camera.main.orthographicSize;
        float screenAspect = (float)Screen.width / Screen.height;
        float orthoWidth = (screenAspect * orthoHeight);

        float pieceWidth = width * gameHolder.localScale.x;
        float pieceHeight = height * gameHolder.localScale.y;

        orthoHeight -= pieceHeight;
        orthoWidth -= pieceWidth;

        foreach (Transform piece in pieces)
        {
            Vector3 startPosition = piece.position;

            float x = Random.Range(-orthoWidth, orthoWidth);
            float y = Random.Range(-orthoHeight, orthoHeight);
            Vector3 targetPosition = new Vector3(x, y, -1);

            StartCoroutine(MovePiece(piece, startPosition, targetPosition, 0.65f)); 
        }

        StartCoroutine(soundEffect.PlaySFXDelay(sfxAudioSource, puzzleScatter, 0.35f));
    }

    private IEnumerator MovePiece(Transform piece, Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            piece.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null; 
        }

        piece.position = targetPosition;
    }


    public void UpdateBorder(Texture2D jigsawTexture)
    {
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();

        float halfWidth = (width * dimensions.x) / 2f;
        float halfHeight = (height * dimensions.y) / 2f;
        float borderZ = 0f;

        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, borderZ));
        lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, borderZ));

        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;

        lineRenderer.enabled = true;

        if (hintQuad != null)
        {
            Destroy(hintQuad);
        }

        hintQuad = new GameObject("HintBackground");
        hintQuad.transform.parent = gameHolder;
        hintQuad.transform.localPosition = new Vector3(0, 0, borderZ + 0.1f);
        hintQuad.transform.localScale = new Vector3(width * dimensions.x, height * dimensions.y, 1f);

        MeshRenderer meshRenderer = hintQuad.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = hintQuad.AddComponent<MeshFilter>();
        meshFilter.mesh = CreateQuadMesh();

        Material hintMaterial = new Material(Shader.Find("Standard"));
        hintMaterial.mainTexture = jigsawTexture;

        hintMaterial.SetFloat("_Mode", 3);
        hintMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        hintMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        hintMaterial.SetInt("_ZWrite", 0);
        hintMaterial.DisableKeyword("_ALPHATEST_ON");
        hintMaterial.EnableKeyword("_ALPHABLEND_ON");
        hintMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        hintMaterial.renderQueue = 3000;

        float alphaValue = 150f / 255f;
        Color albedoColor = new Color(1f, 1f, 1f, alphaValue);
        hintMaterial.SetColor("_Color", albedoColor);

        meshRenderer.material = hintMaterial;
    }

    private void SnapAndDisableIfCorrect()
    {
        int pieceIndex = pieces.IndexOf(draggingPiece);

        int col = pieceIndex % dimensions.x;
        int row = pieceIndex / dimensions.y;

        Vector2 targetPosistion = new((-width * dimensions.x / 2) + (width * col) + (width / 2),
            (-height * dimensions.y / 2) + (height * row) + (height / 2));

        if (Vector2.Distance(draggingPiece.localPosition, targetPosistion) < (width / 2))
        {
            draggingPiece.localPosition = targetPosistion;
            draggingPiece.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(soundEffect.PlaySFXDelay(sfxAudioSource, puzzleSnap, 0.1f));
            piecesCorrect++;
        }


        if (piecesCorrect == pieces.Count)
        {
            starParticle.SetActive(true);
            StartCoroutine(soundEffect.PlaySFXDelay(sfxAudioSource, puzzleDone, 0.5f));
        }

    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[] {
        new Vector3(-0.5f, -0.5f, 0),
        new Vector3(0.5f, -0.5f, 0),
        new Vector3(-0.5f, 0.5f, 0),
        new Vector3(0.5f, 0.5f, 0),
        };

        mesh.uv = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        };

        mesh.triangles = new int[] {
        0, 2, 1,
        2, 3, 1
        };

        return mesh;
    }

    public void RestartGame()
    {
        foreach (Transform piece in pieces)
        {
            Destroy(piece.gameObject);
        }

        pieces.Clear();
        gameHolder.GetComponent<LineRenderer>().enabled = false;
        levelSelectPanel.gameObject.SetActive(true);
        starParticle.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameStateToLevelSelector();
        }

        if (hintQuad != null)
        {
            Destroy(hintQuad);
            hintQuad = null; 
        }

    }

}