using Cook_lib;
using System.IO;
using System.Collections.Generic;
using Connection;

internal class BattleUnit
{
    private UnitBase mPlayer;
    private UnitBase oPlayer;

    private Cook_server cookServer;

    private bool isVsAi;

    internal BattleUnit()
    {
        cookServer = new Cook_server();

        cookServer.ServerSetCallBack(SendData, BattleOver);
    }

    internal void Init(UnitBase _mPlayer, UnitBase _oPlayer, IList<int> _mDish, IList<int> _oDish, bool _isVsAi)
    {
        mPlayer = _mPlayer;
        oPlayer = _oPlayer;

        isVsAi = _isVsAi;

        cookServer.ServerStart(_mDish, _oDish);
    }

    internal void ReceiveData(UnitBase _playerUnit, BinaryReader _br, long _tick)
    {
        bool isMine = _playerUnit == mPlayer;

        cookServer.ServerGetPackage(isMine, _br);
    }

    private void SendData(bool _isMine, bool _isPush, MemoryStream _ms)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                if (_isPush)
                {
                    bw.Write(true);
                }

                bw.Write(_ms.GetBuffer(), 0, (int)_ms.Length);

                if (_isMine)
                {
                    mPlayer.SendData(_isPush, ms);
                }
                else if (oPlayer != null)
                {
                    oPlayer.SendData(_isPush, ms);
                }
            }
        }
    }

    private void BattleOver(GameResult _result)
    {
        mPlayer = oPlayer = null;

        BattleManager.Instance.BattleOver(this);
    }

    internal void Logout(UnitBase _playerUnit)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(PackageTag.C2S_QUIT);

                ms.Position = 0;

                using (BinaryReader br = new BinaryReader(ms))
                {
                    cookServer.ServerGetPackage(_playerUnit == mPlayer, br);
                }
            }
        }
    }

    internal void Update()
    {
        cookServer.ServerUpdateTo();
    }
}
