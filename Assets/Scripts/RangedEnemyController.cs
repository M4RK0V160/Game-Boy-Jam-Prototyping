
using UnityEngine;

public class RangedEnemyController : EnemyController
{

    public GameObject bulletPrefab;
    public AudioClip shootAudio;
    private bool ranged = false;

    public override void Attack()
    {
        audioSource.PlayOneShot(shootAudio);
        gameObject.GetComponent<Animator>().SetTrigger("attack");
        var bullet = Instantiate(bulletPrefab, occupiedCell.GetPositionCenter(), new Quaternion(0, 0, 0, 0));
        bullet.GetComponent<BulletController>().shootingAtEnemy = false;
        bullet.GetComponent<BulletController>().right = facingRight;
        bullet.GetComponent<BulletController>().initialize();
        
    }

    public override bool CheckRange()
    {
        if (Aggro) {
            for (int i = -3; i < 0; i++)
            {
                if (mapManager.cells[occupiedCell.GetPosition().x + i, occupiedCell.GetPosition().y] == playerController.occupiedCell)
                {
                    Debug.Log("hitPlayer");
                    if (facingRight)
                    {
                        gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        facingRight = false;
                    }
                    return true;
                }

                else if (!mapManager.cells[occupiedCell.GetPosition().x + i, occupiedCell.GetPosition().y].IsWalkable())
                {
                    Debug.Log("hitWallLeft");
                    break;

                }
               
            }
            for (int i = 1; i < 4; i++)
            {
                if (mapManager.cells[occupiedCell.GetPosition().x + i, occupiedCell.GetPosition().y] == playerController.occupiedCell)
                {
                    Debug.Log("hitPlayer");
                    if (!facingRight)
                    {
                        gameObject.GetComponent<SpriteRenderer>().flipX = false;
                        facingRight = true;
                    }
                    return true;
                }
                else if (!mapManager.cells[occupiedCell.GetPosition().x + i, occupiedCell.GetPosition().y].IsWalkable())
                {
                    Debug.Log("hitWallRight");
                    break;
                }
                
            }
        }
        return false;
    }

}
