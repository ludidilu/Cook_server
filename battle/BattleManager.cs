using System.Collections.Generic;
using System.IO;
using Connection;
using System;

internal class BattleManager
{
    enum PlayerState
    {
        FREE,
        SEARCHING,
        BATTLE
    }

    internal enum PackageData
    {
        PVP,
        PVE,
        CANCEL_SEARCH
    }

    private static BattleManager _Instance;

    internal static BattleManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new BattleManager();
            }

            return _Instance;
        }
    }

    private Queue<BattleUnit> battleUnitPool = new Queue<BattleUnit>();

    private Dictionary<BattleUnit, List<UnitBase>> battleList = new Dictionary<BattleUnit, List<UnitBase>>();

    private Dictionary<UnitBase, BattleUnit> battleListWithPlayer = new Dictionary<UnitBase, BattleUnit>();

    private List<BattleUnit> delList = new List<BattleUnit>();

    private UnitBase lastPlayer = null;

    internal byte[] Login(UnitBase _playerUnit)
    {
        PlayerState playerState;

        if (_playerUnit == lastPlayer)
        {
            playerState = PlayerState.SEARCHING;
        }
        else
        {
            if (battleListWithPlayer.ContainsKey(_playerUnit))
            {
                playerState = PlayerState.BATTLE;
            }
            else
            {
                playerState = PlayerState.FREE;
            }
        }

        return BitConverter.GetBytes((short)playerState);
    }

    internal void Logout(UnitBase _playerUnit)
    {
        if (lastPlayer == _playerUnit)
        {
            lastPlayer = null;
        }
        else
        {
            BattleUnit unit;

            if (battleListWithPlayer.TryGetValue(_playerUnit, out unit))
            {
                unit.Logout(_playerUnit);
            }
        }
    }

    internal void ReceiveData(UnitBase _playerUnit, byte[] _bytes, long _tick)
    {
        using (MemoryStream ms = new MemoryStream(_bytes))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                bool isBattle = br.ReadBoolean();

                if (isBattle)
                {
                    if (battleListWithPlayer.ContainsKey(_playerUnit))
                    {
                        battleListWithPlayer[_playerUnit].ReceiveData(_playerUnit, br, _tick);
                    }
                    else
                    {
                        if (_playerUnit == lastPlayer)
                        {
                            ReplyClient(_playerUnit, false, PlayerState.SEARCHING);
                        }
                        else
                        {
                            ReplyClient(_playerUnit, false, PlayerState.FREE);
                        }
                    }
                }
                else
                {
                    if (battleListWithPlayer.ContainsKey(_playerUnit))
                    {
                        ReplyClient(_playerUnit, false, PlayerState.BATTLE);
                    }
                    else
                    {
                        PackageData data = (PackageData)br.ReadInt16();

                        ReceiveActionData(_playerUnit, data, _tick);
                    }
                }
            }
        }
    }

    private void ReceiveActionData(UnitBase _playerUnit, PackageData _data, long _tick)
    {
        BattleUnit battleUnit;

        IList<int> mCards;

        IList<int> oCards;

        switch (_data)
        {
            case PackageData.PVP:

                if (lastPlayer == null)
                {
                    lastPlayer = _playerUnit;

                    ReplyClient(_playerUnit, false, PlayerState.SEARCHING);
                }
                else if (lastPlayer == _playerUnit)
                {
                    ReplyClient(_playerUnit, false, PlayerState.SEARCHING);
                }
                else
                {
                    battleUnit = GetBattleUnit();

                    UnitBase tmpPlayer = lastPlayer;

                    lastPlayer = null;

                    battleListWithPlayer.Add(_playerUnit, battleUnit);

                    battleListWithPlayer.Add(tmpPlayer, battleUnit);

                    battleList.Add(battleUnit, new List<UnitBase>() { _playerUnit, tmpPlayer });

                    mCards = new List<int>() { 1, 2, 3, 4, 5 };

                    oCards = new List<int>() { 1, 2, 3, 4, 5 };

                    battleUnit.Init(_playerUnit, tmpPlayer, mCards, oCards, false);

                    ReplyClient(_playerUnit, false, PlayerState.BATTLE);

                    ReplyClient(tmpPlayer, true, PlayerState.BATTLE);
                }

                break;

            case PackageData.PVE:

                if (lastPlayer == _playerUnit)
                {
                    lastPlayer = null;
                }

                battleUnit = GetBattleUnit();

                battleListWithPlayer.Add(_playerUnit, battleUnit);

                battleList.Add(battleUnit, new List<UnitBase>() { _playerUnit });

                mCards = new List<int>();

                oCards = new List<int>();

                battleUnit.Init(_playerUnit, null, mCards, oCards, true);

                ReplyClient(_playerUnit, false, PlayerState.BATTLE);

                break;

            case PackageData.CANCEL_SEARCH:

                if (lastPlayer == _playerUnit)
                {
                    lastPlayer = null;
                }

                ReplyClient(_playerUnit, false, PlayerState.FREE);

                break;
        }
    }

    private void ReplyClient(UnitBase _playerUnit, bool _isPush, PlayerState _playerState)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                if (_isPush)
                {
                    bw.Write(false);
                }

                bw.Write((short)_playerState);

                _playerUnit.SendData(_isPush, ms);
            }
        }
    }

    private BattleUnit GetBattleUnit()
    {
        BattleUnit battleUnit;

        if (battleUnitPool.Count > 0)
        {
            battleUnit = battleUnitPool.Dequeue();
        }
        else
        {
            battleUnit = new BattleUnit();
        }

        return battleUnit;
    }

    private void ReleaseBattleUnit(BattleUnit _battleUnit)
    {
        battleUnitPool.Enqueue(_battleUnit);
    }

    internal void Update()
    {
        IEnumerator<BattleUnit> enumerator = battleList.Keys.GetEnumerator();

        while (enumerator.MoveNext())
        {
            bool b = enumerator.Current.Update();

            if (b)
            {
                delList.Add(enumerator.Current);
            }
        }

        if (delList.Count > 0)
        {
            for (int i = 0; i < delList.Count; i++)
            {
                BattleOver(delList[i]);
            }

            delList.Clear();
        }
    }

    private void BattleOver(BattleUnit _battleUnit)
    {
        List<UnitBase> tmpList = battleList[_battleUnit];

        for (int i = 0; i < tmpList.Count; i++)
        {
            battleListWithPlayer.Remove(tmpList[i]);
        }

        battleList.Remove(_battleUnit);

        ReleaseBattleUnit(_battleUnit);
    }
}