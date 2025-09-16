using UnityEngine;
using System.Collections;

public class EscapeAnimalAction : MonoBehaviour
{
    //=============================================
    public bool IsFree = false;  //�������܂���o�Ă��邩�ǂ���
    public float MoveSpeed = 0.1f;
    public float EscapeSpeed = 6.0f;
    public float ChangeInterval = 3.0f;
    private Vector3 moveDirection;
    //==============================================
    // �O��̍s���itrue=�ړ�, false=��~�j
    private bool wasMoving = false;
    // ���̍s���܂ł̎c�莞��
    private float timer;
    //==============================================
    // �s�����J�n����͈͂̒��S�_
    //public Transform CenterPoint;
    // �s�����J�n����͈͂̔��a
    public float EscapeRadius = 15.0f;
    // Goal�I�u�W�F�N�g��Transform
    private Transform goalTransform;
    // �E�o�����������������ǂ����̃t���O
    private bool hasEscaped = false;
    //==============================================
    // ���g��j�����鋗��
    public float destructionDistance = 2.0f;
    //==============================================

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //=================================================================
        // �����^�C�}�[��ݒ�
        timer = ChangeInterval;
        //=================================================================
        // "Goal"�^�O�̃I�u�W�F�N�g��T���Ď擾
        GameObject goalObject = GameObject.FindGameObjectWithTag("Goal");
        if (goalObject != null)
        {
            goalTransform = goalObject.transform;
        }
        else
        {
            Debug.LogError("Goal�I�u�W�F�N�g��������܂���B");
        }
    }
    // Update is called once per frame
    void Update()
    {
        //�������B����o�Ă��Ȃ��Ȃ�
        if (!IsFree)
        {
            CageAnimalAct();
        }
        else
        {
            // �B����o�āA����Goal�I�u�W�F�N�g�����݂���ꍇ
            if (goalTransform != null)
            {
                // �������ĂȂ��ꍇ�B����̒E�o����
                if (!hasEscaped)
                {
                    hasEscaped = true;
                    // EscapeFromRange();
                }
                // ������Ă�ꍇ�S�[���ւ̒ǐՏ���
                else
                {
                    MoveToGoal();
                }
            }
        }
        //�S�[���ɓ��������玩�g��j�����鏈��
        MoveToDestory();
    }

    // �w��͈͂���̒E�o����
    private void EscapeFromRange()
    {
        // ���S�_����O�փ��[�v���郉���_���ȕ������v�Z
        Vector3 randomDirection = Random.onUnitSphere;
        // Y���̕�����0�ɂ��Ēn�ʂ��ړ�����悤�ɂ���
        randomDirection.y = 0;
        randomDirection.Normalize();

        // ���S�_����w�肳�ꂽ���a�̊O���Ƀ��[�v����ʒu������
        //Vector3 warpPosition = CenterPoint.position + randomDirection * EscapeRadius;
        Vector3 warpPosition = transform.position + randomDirection * EscapeRadius;

        // �I�u�W�F�N�g��V�����ʒu�֏u�Ԉړ�������
        transform.position = new Vector3(warpPosition.x, 1.5f, warpPosition.z);

        // ���[�v�����t���O�𗧂Ă�
        IsFree = true;
        hasEscaped = true;
        Debug.Log("�͈͊O�ւ̃��[�v���������܂����B");
    }

    // Goal�I�u�W�F�N�g�ւ̈ړ�����
    private void MoveToGoal()
    {
        // Goal�ւ̕������v�Z
        Vector3 directionToGoal = (goalTransform.position - transform.position).normalized;
        // ���̕����Ɉړ�
        transform.position += directionToGoal * EscapeSpeed * Time.deltaTime;
    }

    //�S�[���ɒ������玩�g��j�����鏈��
    void MoveToDestory()
    {
        // ���g�ƃS�[���I�u�W�F�N�g�̋������v�Z
        float distanceToGoal = Vector3.Distance(transform.position, goalTransform.position);

        // �������w�肵���l�ȉ��ɂȂ����玩�g��j��
        if (distanceToGoal <= destructionDistance)
        {
            // ���g��j��
            Destroy(gameObject);
            Debug.Log("�S�[���ɓ��B�������߁A�I�u�W�F�N�g��j�����܂����B");
        }
    }

    private void DecideNextAction()
    {
        // ���̍s�����u�ړ��v�ɂȂ�m��
        float moveChance = 0.5f;

        // �����O��̍s�����u��~�v�������ꍇ�A���̍s�����u�ړ��v�ɂȂ�m������������
        if (!wasMoving)
        {
            moveChance = 0.8f; // ��F��~�̌��80%�̊m���ňړ�
        }
        // �m���Ɋ�Â��čs��������
        if (Random.value < moveChance)
        {
            // �ړ�����ꍇ
            // �V���������_���Ȉړ�����������
            moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            wasMoving = true;
            //Debug.Log("�ړ����܂�"); //��U�����܂�(Zan)
        }
        else
        {
            // ��~����ꍇ
            // �ړ��������[���ɂ���
            moveDirection = Vector3.zero;
            wasMoving = false;
            //Debug.Log("��~���܂�"); //��U�����܂�(Zan)
        }
    }
    void CageAnimalAct()
    {
        // �^�C�}�[�����炷
        timer -= Time.deltaTime;

        // �^�C�}�[��0�ȉ��ɂȂ�����V�����s�������肷��
        if (timer <= 0)
        {
            DecideNextAction();
            // �^�C�}�[�����Z�b�g
            timer = ChangeInterval;
        }
        // ���݂̈ړ������Ɋ�Â��ăI�u�W�F�N�g���ړ�������

        transform.position += moveDirection * MoveSpeed * Time.deltaTime;
    }
    void OnCollisionEnter(Collision collision)
    {
        //"Wall"�ƏՓ˂�����
        // �Փ˂����I�u�W�F�N�g�̃^�O�� "Wall" �ł��邩�`�F�b�N
        if (collision.gameObject.CompareTag("Wall"))
        {
            // X����Z���̈ړ������𔽓]         
            moveDirection.x *= -1.0f;
            moveDirection.z *= -1.0f;
        }
    }
}

