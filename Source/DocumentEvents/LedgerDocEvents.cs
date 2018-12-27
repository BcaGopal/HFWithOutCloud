﻿using CustomEventArgs;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerDocumentEvents
{
    public delegate bool DocumentEventHandlerWithReturn(object sender, LedgerEventArgs EventArgs, ref ApplicationDbContext db);
    public delegate void DocumentEventHandler(object sender, LedgerEventArgs EventArgs, ref ApplicationDbContext db);
    public class LedgerDocEvents
    {       

              //HeaderSave Events
        protected static event DocumentEventHandlerWithReturn _beforeHeaderSave;
        protected static event DocumentEventHandler _onHeaderSave;
        protected static event DocumentEventHandler _afterHeaderSave;

        //Header Delete
        protected static event DocumentEventHandlerWithReturn _beforeHeaderDelete;
        protected static event DocumentEventHandler _onHeaderDelete;
        protected static event DocumentEventHandler _afterHeaderDelete;

        //Header Submit
        protected static event DocumentEventHandlerWithReturn _beforeHeaderSubmit;
        protected static event DocumentEventHandler _onHeaderSubmit;
        protected static event DocumentEventHandler _afterHeaderSubmit;

        //Header Approve
        protected static event DocumentEventHandlerWithReturn _beforeHeaderApprove;
        protected static event DocumentEventHandler _onHeaderApprove;
        protected static event DocumentEventHandler _afterHeaderApprove;

        //Header Print
        protected static event DocumentEventHandlerWithReturn _beforeHeaderPrint;
        protected static event DocumentEventHandler _afterHeaderPrint;

        //Line Save
        protected static event DocumentEventHandlerWithReturn _beforeLineSave;
        protected static event DocumentEventHandler _onLineSave;
        protected static event DocumentEventHandler _afterLineSave;

        //Line Save Bulk
        protected static event DocumentEventHandlerWithReturn _beforeLineSaveBulk;
        protected static event DocumentEventHandler _onLineSaveBulk;
        protected static event DocumentEventHandler _afterLineSaveBulk;

        //Line Delete
        protected static event DocumentEventHandlerWithReturn _beforeLineDelete;
        protected static event DocumentEventHandler _onLineDelete;
        protected static event DocumentEventHandler _afterLineDelete;

        //Wizard Save
        protected static event DocumentEventHandler _onWizardSave;

        public static bool Initialized { get; set; }

        #region Methods to Raise Events from Controller
        //Public methods to call where ever we intend to raise out custom events
        //Calling this method with the arguments will raise the appropriate event and all the subscibers of this event will be noified to execute event data

        public static bool beforeHeaderSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeHeaderSave != null)
                return _beforeHeaderSave(sender, Args, ref db);
            else return true;
        }

        public static void onHeaderSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onHeaderSave != null)
                _onHeaderSave(sender, Args, ref db);
        }

        public static void afterHeaderSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterHeaderSave != null)
                _afterHeaderSave(sender, Args, ref db);
        }

        public static bool beforeHeaderDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeHeaderDelete != null)
                return _beforeHeaderDelete(sender, Args, ref db);
            else return true;
        }

        public static void onHeaderDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onHeaderDelete != null)
                _onHeaderDelete(sender, Args, ref db);
        }

        public static void afterHeaderDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterHeaderDelete != null)
                _afterHeaderDelete(sender, Args, ref db);
        }

        public static bool beforeHeaderSubmitEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeHeaderSubmit != null)
                return _beforeHeaderSubmit(sender, Args, ref db);
            else return true;
        }

        public static void onHeaderSubmitEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onHeaderSubmit != null)
                _onHeaderSubmit(sender, Args, ref db);
        }

        public static void afterHeaderSubmitEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterHeaderSubmit != null)
                _afterHeaderSubmit(sender, Args, ref db);
        }

        public static bool beforeHeaderApproveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeHeaderApprove != null)
                return _beforeHeaderApprove(sender, Args, ref db);
            else return true;
        }

        public static void onHeaderApproveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onHeaderApprove != null)
                _onHeaderApprove(sender, Args, ref db);
        }

        public static void afterHeaderApproveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterHeaderApprove != null)
                _afterHeaderApprove(sender, Args, ref db);
        }


        public static bool beforeHeaderPrintEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeHeaderPrint != null)
                return _beforeHeaderPrint(sender, Args, ref db);
            else return true;
        }

        public static void afterHeaderPrintEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterHeaderPrint != null)
                _afterHeaderPrint(sender, Args, ref db);
        }

        public static bool beforeLineSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeLineSave != null)
                return _beforeLineSave(sender, Args, ref db);
            else return true;
        }

        public static void onLineSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onLineSave != null)
                _onLineSave(sender, Args, ref db);
        }

        public static void afterLineSaveEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterLineSave != null)
                _afterLineSave(sender, Args, ref db);
        }

        public static bool beforeLineSaveBulkEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeLineSaveBulk != null)
                return _beforeLineSaveBulk(sender, Args, ref db);
            else return true;
        }

        public static void onLineSaveBulkEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onLineSaveBulk != null)
                _onLineSaveBulk(sender, Args, ref db);
        }

        public static void afterLineSaveBulkEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterLineSaveBulk != null)
                _afterLineSaveBulk(sender, Args, ref db);
        }


        public static bool beforeLineDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_beforeLineDelete != null)
                return _beforeLineDelete(sender, Args, ref db);
            else return true;
        }

        public static void onLineDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onLineDelete != null)
                _onLineDelete(sender, Args, ref db);
        }

        public static void afterLineDeleteEvent(object sender, LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_afterLineDelete != null)
                _afterLineDelete(sender, Args, ref db);
        }

        public static void onWizardSaveEvent(object sender,LedgerEventArgs Args, ref ApplicationDbContext db)
        {
            if (_onWizardSave != null)
                _onWizardSave(sender, Args, ref db);
        }

        #endregion
    }    

}