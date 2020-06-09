using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using Engine;

namespace qRPG
{
    public partial class qRPG : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public qRPG()
        {
            InitializeComponent();

            
            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
                        
            lblHitPoints.Text = _player.CurrHP.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExp.Text = _player.Exp.ToString();
            lblLevel.Text = _player.Level.ToString(); 

        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToN);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToE);        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToS);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToW);
        }

        private void MoveTo(Location newLocation)
        {
            //check req items
            if(newLocation.ItemRequiredToEnter != null)
            {
                //check if player has item
                bool playerHasRequiredItem = false;

                foreach(InventoryItem ii in _player.Inventory)
                {
                    if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        //giereś juppi
                        playerHasRequiredItem = true;
                        break;
                    }
                }

                if(!playerHasRequiredItem)
                {
                    //message required item
                    rtbMessages.Text += "You must have " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            //update players current location
            _player.CurrentLocation = newLocation;

            // show/hide avaliable buttons
            btnNorth.Visible = (newLocation.LocationToN != null);
            btnEast.Visible = (newLocation.LocationToE != null);
            btnSouth.Visible = (newLocation.LocationToS != null);
            btnWest.Visible = (newLocation.LocationToW != null);

            //display current location
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Desc + Environment.NewLine;

            //heal the player
            _player.CurrHP = _player.MaxHP;

            //update hp in ui
            lblHitPoints.Text = _player.CurrHP.ToString();

            //check if location has quest
            if(newLocation.QuestAvailableHere !=null)
            {
                //check if player already has quest and completed it
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach(PlayerQuest playerQuest in _player.Quests)
                {
                    if(playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if(playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                if(playerAlreadyHasQuest)
                {
                    //if not completed
                    if(!playerAlreadyCompletedQuest)
                    {
                        //check if has items needed
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            //check items in inventory to check if they have it and if they have enough
                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                //player has items
                                if(ii.Quantity == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;

                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        //player doesnt have enough
                                        playerHasAllItemsToCompleteQuest = false;
                                        break;
                                    }
                                    break;

                                }
                            }
                            
                            //if item not found in inventory
                            if (!foundItemInPlayersInventory)
                            {
                                playerHasAllItemsToCompleteQuest = false;
                                break;
                            }
                        }

                        //player has all items to complete quest
                        if(playerHasAllItemsToCompleteQuest)
                        {
                            //display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the '" + newLocation.QuestAvailableHere.Name + "'quest." + Environment.NewLine;

                            //remove quest items form inventory
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        //subtract the quantity required for quest from player inventory
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            //give quest rewards
                            //sprawdź /n
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExp.ToString() + "experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + "gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.Exp += newLocation.QuestAvailableHere.RewardExp;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //add reward item to player inventory
                            bool addedItemToPlayerInventory = false;

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                if(ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    //item is already in inventory, increase quantity by 1
                                    ii.Quantity++;
                                    addedItemToPlayerInventory = true;
                                    break;
                                }
                            }

                            //no item found, add item to inventroy
                            if(!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            
                            //find quest in players quest list
                            foreach(PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    //mark quest as complete
                                    pq.IsCompleted = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //player doesnt have quest

                    //display message
                    rtbMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Desc + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;
                    foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if(qci.Quantity == 1)
                        {
                            rtbMessages.Text +=qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    //add quest to players quest log
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            //check if location have monster
            if(newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //make a new monster instance
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaxDmg, standardMonster.RewardExp, standardMonster.RewardGold, standardMonster.CurrHP, standardMonster.MaxHP);

                foreach(LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;
                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
            }

            //refresh inventory list
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";
            dgvInventory.Rows.Clear();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }

            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Completed?";
            dgvQuests.Rows.Clear();

            foreach(PlayerQuest playerQuest in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }

            //refresh weapon combobox
            List<Weapon> weapons = new List<Weapon>();
            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Details is Weapon)
                {
                    if(inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }

            }

            if(weapons.Count == 0)
            {
                //player doesnt have weapons, hide weapon choice
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;
            }
            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            //refresh potion combobox
            List<HPotion> healingPotions = new List<HPotion>();

            foreach(InventoryItem inventoryItem in _player.Inventory)
            {
                if(inventoryItem.Details is HPotion)
                {
                    if(inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HPotion)inventoryItem.Details);
                    }
                }
            }

            if(healingPotions.Count == 0)
            {
                //no potions, hide combobox and button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "10";

                cboPotions.SelectedIndex = 0;
            }

        }

        
        
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            
        }
    }
}
