using UnityEngine;

public class GameStartUI : MonoBehaviour
{
    //�Q�[���I�[�o�[�A�N���A�Ɠ����悤�ɂς��Ƃ����Ăς��Ə����悤�ɂ���
    [SerializeField] private Vector3 firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private bool isApear = false;  //�������o�������邩
    [SerializeField] private float uiAppearSeconds = 2.0f;//UI�̏o������    
    [SerializeField] private float decelTimeSpeedNDeltaTime = 1.0f;//�o�����Ԃ��������鑬��

    [SerializeField] private GameObject fadeObject;

    private void Awake()
    {
        //�����ʒu�ݒ�
        //Vector3 newPosition = transform.position;
        //newPosition = new Vector3(-(Screen.width * 0.5f), Screen.height * 0.5f, 0.0f);
        //transform.position = newPosition;
        //isAppear = true;

        //UI�������ʒu�ɐݒ�(��ʊO)
        transform.position = firstPosition;
        //�n�܂����u�ԏo�����������̂�
        isApear = true;
    }

    void Start()
    {
        fadeObject.GetComponent<Animator>().Play("FadeIn");
    }

    // Update is called once per frame
    void Update()
    {
        if (isApear)
        {
            //uiAppearFrame -= decelTimeSpeed * Time.deltaTime;//UI�̕\�����Ԃ����炵�Ă���
            uiAppearSeconds -= decelTimeSpeedNDeltaTime * Time.unscaledDeltaTime;//UI�̕\�����Ԃ����炵�Ă���
            Vector3 newPosition = transform.position;        //�I�u�W�F�N�g�̍��W����
            newPosition = new Vector3(Screen.width * 0.5f,
                           Screen.height * 0.5f, 0);         //�錾�����ϐ��Ɍ��_����             
            transform.position = newPosition;                //�V����������ϐ����I�u�W�F�N�g�ɓ���Ȃ���                        
        }
        //UI�̕\�����Ԃ�0�ȉ��ɂȂ�����
        if (uiAppearSeconds <= 0.0f)
        {
            Time.timeScale = 1;//���Ԓ�~����������
            isApear = false;     //�o������������
            Destroy(gameObject); //�I�u�W�F�N�g��j������           
        }
    }
    private void FixedUpdate()
    {
        //UI�̕\�����Ԃ�0���߂Ȃ�
        if (uiAppearSeconds > 0.0f)
        {
            //�o�����Ă���Ԃ̓Q�[���̎��Ԃ��~�߂�
            Time.timeScale = 0;
        }
    }
}
