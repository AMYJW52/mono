/**
 * Namespace: System.Web.UI.WebControls
 * Class:     Calendar
 * 
 * Author:  Gaurav Vaish
 * Maintainer: gvaish@iitk.ac.in
 * Contact: <my_scripts2001@yahoo.com>, <gvaish@iitk.ac.in>
 * Implementation: yes
 * Status:  60%
 * 
 * (C) Gaurav Vaish (2001)
 */

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing;

namespace System.Web.UI.WebControls
{
	public class Calendar : WebControl, IPostBackEventHandler
	{
		//
		
		private TableItemStyle          dayHeaderStyle;
		private TableItemStyle          dayStyle;
		private TableItemStyle          nextPrevStyle;
		private TableItemStyle          otherMonthDayStyle;
		private SelectedDatesCollection selectedDates;
		private ArrayList               selectedDatesList;
		private TableItemStyle          selectedDayStyle;
		private TableItemStyle          selectorStyle;
		private TableItemStyle          titleStyle;
		private TableItemStyle          todayDayStyle;
		private TableItemStyle          weekendDayStyle;

		private static readonly object DayRenderEvent           = new object();
		private static readonly object SelectionChangedEvent    = new object();
		private static readonly object VisibleMonthChangedEvent = new object();
		
		private Color defaultTextColor;
		private System.Globalization.Calendar globCal;

		public Calendar(): base()
		{
			//TODO: Initialization
		}
		
		public int CellPadding
		{
			get
			{
				object o = ViewState["CellPadding"];
				if(o!=null)
					return (int)o;
				return 2;
			}
			set
			{
				ViewState["CellPadding"] = value;
			}
		}

		public int CellSpacing
		{
			get
			{
				object o = ViewState["CellSpacing"];
				if(o!=null)
					return (int)o;
				return 0;
			}
			set
			{
				if(value<-1)
					throw new ArgumentOutOfRangeException();
				ViewState["CellSpacing"] = value;
			}
		}
		
		public TableItemStyle DayHeaderStyle
		{
			get
			{
				if(dayHeaderStyle==null)
					dayHeaderStyle = new TableItemStyle();
				if(IsTrackingViewState)
					dayHeaderStyle.TrackViewState();
				return dayHeaderStyle;
			}
		}

		public DayNameFormat DayNameFormat
		{
			get
			{
				object o = ViewState["DayNameFormat"];
				if(o!=null)
					return (DayNameFormat)o;
				return DayNameFormat.Short;
			}
			set
			{
				if(!System.Enum.IsDefined(typeof(DayNameFormat),value))
					throw new ArgumentException();
				ViewState["DayNameFormat"] = value;
			}
		}

		public TableItemStyle DayStyle
		{
			get
			{
				if(dayStyle==null)
					dayStyle = new TableItemStyle();
				if(IsTrackingViewState)
					dayStyle.TrackViewState();
				return dayStyle;
			}
		}
		
		public FirstDayOfWeek FirstDayOfWeek
		{
			get
			{
				object o = ViewState["FirstDayOfWeek"];
				if(o!=null)
					return (FirstDayOfWeek)o;
				return FirstDayOfWeek.Default;
			}
			set
			{
				if(!System.Enum.IsDefined(typeof(FirstDayOfWeek), value))
					throw new ArgumentException();
				ViewState["FirstDayOfWeek"] = value;
			}
		}
		
		public string NextMonthText
		{
			get
			{
				object o = ViewState["NextMonthText"];
				if(o!=null)
					return (string)o;
				return "&gt;";
			}
			set
			{
				ViewState["NextMonthText"] = value;
			}
		}
		
		public NextPrevFormat NextPrevFormat
		{
			get
			{
				object o = ViewState["NextPrevFormat"];
				if(o!=null)
					return (NextPrevFormat)o;
				return NextPrevFormat.CustomText;
			}
			set
			{
				if(!System.Enum.IsDefined(typeof(NextPrevFormat), value))
					throw new ArgumentException();
				ViewState["NextPrevFormat"] = value;
			}
		}
		
		public TableItemStyle NextPrevStyle
		{
			get
			{
				if(nextPrevStyle == null)
					nextPrevStyle = new TableItemStyle();
				if(IsTrackingViewState)
					nextPrevStyle.TrackViewState();
				return nextPrevStyle;
			}
		}
		
		public TableItemStyle OtherMonthDayStyle
		{
			get
			{
				if(otherMonthDayStyle == null)
					otherMonthDayStyle = new TableItemStyle();
				if(IsTrackingViewState)
					otherMonthDayStyle.TrackViewState();
				return otherMonthDayStyle;
			}
		}
		
		public string PrevMonthText
		{
			get
			{
				object o = ViewState["PrevMonthText"];
				if(o!=null)
					return (string)o;
				return "&lt;";
			}
			set
			{
				ViewState["PrevMonthText"] = value;
			}
		}
		
		public DateTime SelectedDate
		{
			get
			{
				if(SelectedDates.Count > 0)
				{
					return SelectedDates[0];
				}
				return DateTime.MinValue;
			}
			set
			{
				if(value == DateTime.MinValue)
				{
					SelectedDates.Clear();
				} else
				{
					SelectedDates.SelectRange(value, value);
				}
			}
		}
		
		public SelectedDatesCollection SelectedDates
		{
			get
			{
				if(selectedDates==null)
				{
					if(selectedDatesList == null)
						selectedDatesList = new ArrayList();
					selectedDates = new SelectedDatesCollection(selectedDatesList);
				}
				return selectedDates;
			}
		}
		
		public TableItemStyle SelectedDayStyle
		{
			get
			{
				if(selectedDayStyle==null)
					selectedDayStyle = new TableItemStyle();
				if(IsTrackingViewState)
					selectedDayDtyle.TrackViewState();
				return selectedDayStyle;
			}
		}

		public CalendarSelectionMode SelectionMode
		{
			get
			{
				object o = ViewState["SelectionMode"];
				if(o!=null)
					return (CalendarSelectionMode)o;
				return CalendarSelectionMode.Day;
			}
			set
			{
				if(!System.Enum.IsDefined(typeof(CalendarSelectionMode), value))
					throw new ArgumentException();
				ViewState["SelectionMode"] = value;
			}
		}
		
		public string SelectedMonthText
		{
			get
			{
				object o = ViewState["SelectedMonthText"];
				if(o!=null)
					return (string)o;
				return "&gt;&gt;";
			}
			set
			{
				ViewState["SelectedMonthText"] = value;
			}
		}

		public TableItemStyle SelectorStyle
		{
			get
			{
				if(selectorStyle==null)
					selectorStyle = new TableItemStyle();
				return selectorStyle;
			}
		}
		
		public string SelectWeekText
		{
			get
			{
				object o = ViewState["SelectWeekText"];
				if(o!=null)
					return (string)o;
				return "&gt;";
			}
			set
			{
				ViewState["SelectWeekText"] = value;
			}
		}
		
		public bool ShowDayHeader
		{
			get
			{
				object o = ViewState["ShowDayHeader"];
				if(o!=null)
					return (bool)o;
				return true;
			}
			set
			{
				ViewState["ShowDayHeader"] = value;
			}
		}
		
		public bool ShowGridLines
		{
			get
			{
				object o = ViewState["ShowGridLines"];
				if(o!=null)
					return (bool)o;
				return false;
			}
			set
			{
				ViewState["ShowGridLines"] = value;
			}
		}
		
		public bool ShowNextPrevMonth
		{
			get
			{
				object o = ViewState["ShowNextPrevMonth"];
				if(o!=null)
					return (bool)o;
				return true;
			}
			set
			{
				ViewState["ShowNextPrevMonth"] = value;
			}
		}
		
		public bool ShowTitle
		{
			get
			{
				object o = ViewState["ShowTitle"];
				if(o!=null)
					return (bool)o;
				return true;
			}
			set
			{
				ViewState["ShowTitle"] = value;
			}
		}

		public TitleFormat TitleFormat
		{
			get
			{
				object o = ViewState["TitleFormat"];
				if(o!=null)
					return (TitleFormat)o;
				return TitleFormat.MonthYear;
			}
			set
			{
				if(!System.Enum.IsDefined(typeof(TitleFormat), value))
					throw new ArgumentException();
				ViewState["TitleFormat"] = value;
			}
		}
		
		public TableItemStyle TitleStyle
		{
			get
			{
				if(titleStyle==null)
					titleStyle = new TableItemStyle();
				if(IsTrackingViewState)
					titleStyle.TrackViewState();
				return titleStyle;
			}
		}
		
		public TableItemStyle TodayDayStyle
		{
			get
			{
				if(todayDayStyle==null)
					todayDayStyle = new TableItemStyle();
				if(IsTrackingViewState)
					todayDayStyle.TrackViewState();
				return todayDayStyle;
			}
		}
		
		public DateTime TodaysDate
		{
			get
			{
				object o = ViewState["TodaysDate"];
				if(o!=null)
					return (DateTime)o;
				return DateTime.Today;
			}
			set
			{
				ViewState["TodaysDate"] = value;
			}
		}
		
		public DateTime VisibleDate
		{
			get
			{
				object o = ViewState["VisibleDate"];
				if(o!=null)
					return (DateTime)o;
				return DateTime.MinValue;
			}
			set
			{
				ViewState["VisibleDate"] = value;
			}
		}
		
		public TableItemStyle WeekendDayStyle
		{
			get
			{
				if(weekendDayStyle == null)
					weekendDayStyle = new TableItemStyle();
				if(IsTrackingViewState)
				{
					weekendDayStyle.TrackViewState();
				}
				return weekendDayStyle;
			}
		}
		
		public event DayRenderEventHandler DayRender
		{
			add
			{
				Events.AddHandler(DayRenderEvent, value);
			}
			remove
			{
				Events.RemoveHandler(DayRenderEvent, value);
			}
		}
		
		public event EventHandler SelectionChanged
		{
			add
			{
				Events.AddHandler(SelectionChangedEvent, value);
			}
			remove
			{
				Events.RemoveHandler(SelectionChangedEvent, value);
			}
		}

		public event MonthChangedEventHandler VisibleMonthChanged
		{
			add
			{
				Events.AddHandler(VisibleMonthChangedEvent, value);
			}
			remove
			{
				Events.RemoveHandler(VisibleMonthChangedEvent, value);
			}
		}
		
		protected virtual void OnDayRender(TableCell cell, CalendarDay day)
		{
			if(Events!=null)
			{
				DayRenderEventHandler dreh = (DayRenderEventHandler)(Events[DayRenderEvent]);
				if(dreh!=null)
					dreh(this, new DayRenderEventArgs(cell, day));
			}
		}
		
		protected virtual void OnSelectionChanged()
		{
			if(Events!=null)
			{
				EventHandler eh = (EventHandler)(Events[SelectionChangedEvent]);
				if(eh!=null)
					eh(this, new EventArgs());
			}
		}
		
		protected virtual void OnVisibleMonthChanged(DateTime newDate, DateTime prevDate)
		{
			if(Events!=null)
			{
				MonthChangedEventHandler mceh = (MonthChangedEventHandler)(Events[VisibleMonthChangedEvent]);
				if(mceh!=null)
					mceh(this, new MonthChangedEventArgs(newDate, prevDate));
			}
		}

		/// <remarks>
		/// See test6.aspx in Tests directory for verification
		/// </remarks>
		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
		{
			globCal = DateTimeFormatInfo.CurrentInfo.Calendar;
			DateTime visDate = GetEffectiveVisibleDate();
			//FIXME: Should it be String.Compare(eventArgument, "nextMonth", false);
			if(eventArgument == "nextMonth")
			{
				VisibleDate = globCal.AddMonths(visDate, 1);
				OnVisibleDateChanged(VisibleDate, visDate);
				return;
			}
			if(eventArgument == "prevMonth")
			{
				VisibleDate = globCal.AddMonths(visDate, -1);
				OnVisibleDateChanged(VisibleDate, visDate);
				return;
			}
			if(eventArgument == "selectMonth")
			{
				DateTime oldDate = new DateTime(globCal.GetYear(visDate), globCal.GetMonth(visDate), 1, globCal);
				SelectRangeInternal(oldDate, globCal.AddDays(gloCal.AddMonths(oldDate, 1), -1), visDate);
				return;
			}
			if(String.Compare(eventArgument, 0, "selectWeek", 0, "selectWeek".Length)==0)
			{
				int week = -1;
				try
				{
					week = Int32.Parse(eventArgument.Substring("selectWeek".Length));
				} catch(Exception e)
				{
				}
				if(week >= 0 && week <= 5)
				{
					DateTime weekStart = globCal.AddDays(GetFirstCalendarDay(visDate), week * 7);
					SelectRangeInternal(weekStart, globCal.AddDays(weekStart, 6), visDate);
				}
				return;
			}
			if(String.Compare(eventArgument, 0, "selectDay", 0, "selectDay".Length)==0)
			{
				int day = -1;
				try
				{
					day = Int32.Parse(eventArgument.Substring("selectDay".Length);
				} catch(Exception e)
				{
				}
				if(day >= 0 && day <= 42)
				{
					DateTime dayStart = globCal.AddDays(GetFirstCalendarDay(visDate), day);
					SelectRangeInternal(dayStart, dayStart, visDate);
				}
			}
		}
		
		protected override void Render(HtmlTextWriter writer)
		{
			//TODO: Implement me
			throw new NotImplementedException();
			globCal = DateTimeFormatInfo.CurrentInfo.Calendar;
			DateTime visDate   = GetEffectiveVisibleDate()
			DateTime firstDate = GetFirstCalendarDay(visDate);
			
			/*
			 * ForeColor else defaultTextColor
			 * Draw a table
			 * if(ControlStyleCreated())
			 * 	then
			 * {
			 *	 ApplyStyle(ControlStyle)
			 * }
			 * GridLines?
			 * RenderBeginTag(writer)
			 * RenderTitle(writer, visibleDate from GetEffectiveVisibleDate, this.SelectionMode, IsEnabled)
			 * if(ShowHeader)
			 *  RenderHeader(writer, visibleDate, SelectionMode, IsEnabled, 
			 * RenderAllDays
			 * RenderEndTag(writer)
			 */
		}
		
		protected override ControlCollection CreateControlCollection()
		{
			return new EmptyControlCollection(this);
		}
		
		protected override void LoadViewState(object savedState)
		{
			if(savedState!=null)
			{
				if(ViewState["_CalendarSelectedDates"] != null)
					SelectedDates = (SelectedDatesCollection)ViewState["_CalendarSelectedDates"];

				object[] states = (object[]) savedState;
				if(states[0] != null)
					base.LoadViewState(states[0]);
				if(states[1] != null)
					DayHeaderStyle.LoadViewState(states[1]);
				if(states[2] != null)
					DayStyle.LoadViewState(states[2]);
				if(states[3] != null)
					NextPrevStyle.LoadViewState(states[3]);
				if(states[4] != null)
					OtherMonthStyle.LoadViewState(states[4]);
				if(states[5] != null)
					SelectedDayStyle.LoadViewState(states[5]);
				if(states[6] != null)
					SelectorStyle.LoadViewState(states[6]);
				if(states[7] != null)
					TitleStyle.LoadViewState(states[7]);
				if(states[8] != null)
					TodayDayStyle.LoadViewState(states[8]);
				if(states[9] != null)
					WeekendDayStyle.LoadViewState(states[9]);
			}
		}
		
		protected override object SaveViewState()
		{
			ViewState["_CalendarSelectedDates"] = (SelectedDates.Count > 0 ? selectedDates : null);
			object[] states = new object[11];
			states[0] = base.SaveViewState();
			states[1] = (dayHeaderStyle == null ? null : dayHeaderStyle.SaveViewStyle());
			states[2] = (dayStyle == null ? null : dayStyle.SaveViewStyle());
			states[3] = (nextPrevStyle == null ? null : nextPrevStyle.SaveViewStyle());
			states[4] = (otherMonthDayStyle == null ? null : otherMonthDayStyle.SaveViewStyle());
			states[5] = (selectedDayStyle == null ? null : selectedDayStyle.SaveViewStyle());
			states[6] = (selectorStyle == null ? null : selectorStyle.SaveViewStyle());
			states[7] = (titleStyle == null ? null : titleStyle.SaveViewStyle());
			states[8] = (todayDayStyle == null ? null : todayDayStyle.SaveViewStyle());
			states[9] = (weekendDayStyle == null ? null : weekendDayStyle.SaveViewStyle());
			for(int i=0; i < states.Length)
			{
				if(states[i]!=null)
					return states;
			}
			return null;
		}
		
		protected override void TrackViewState(): TrackViewState()
		{
			if(titleStyle!=null)
			{
				titleStyle.TrackViewState();
			}
			if(nextPrevStyle!=null)
			{
				nextPrevStyle.TrackViewState();
			}
			if(dayStyle!=null)
			{
				dayStyle.TrackViewState();
			}
			if(dayHeaderStyle!=null)
			{
				dayHeaderStyle.TrackViewState();
			}
			if(todayDayStyle!=null)
			{
				todayDayStyle.TrackViewState();
			}
			if(weekendDayStyle!=null)
			{
				weekendDayStyle.TrackViewState();
			}
			if(otherMonthDayStyle!=null)
			{
				otherMonthDayStyle.TrackViewState();
			}
			if(selectedDayStyle!=null)
			{
				selectedDayStyle.TrackViewState();
			}
			if(selectorStyle!=null)
			{
				selectorStyle.TrackViewState();
			}
		}
		
		private void RenderAllDays(HtmlTextWriter writer, DateTime firstDay, DateTime activeDate, CalendarSelectionMode mode, bool isActive, bool isDownLevel)
		{
			throw new NotImplementedException();
			//TODO: Implement me
			/*
			 * "<tr>"
			 * "</tr>"
			 */
		}
		
		private void RenderHeader(HtmlTextWriter writer, DateTime firstDay, CalendarSelectionMode mode, bool isActive, bool isDownLevel)
		{
			throw new NotImplementedException();
			//TODO: Implement Me
			/*
			 * "<tr>"
			 * "</tr>"
			 */
		}
		
		private void RenderTitle(HtmlTextWriter writer, DateTime visibleDate, CalendarSelectionMode mode, bool isActive)
		{
			throw new NotImplementedException();
			//TODO: Implement me
			/*
			 * Make a row with the following contents: "<tr>"
			 * Draw a table, with cell having the following properties
			 * if(mode==CalendarSelectionMode.DayWeek || mode==CalendarSelectionMode.DayWeekMonth)
			 *	then draw a column with colspan=8
			 * else
			 *	draw with colspan=7
			 * set gridlines?
			 * set width
			 * set cellspacing
			 * ApplyStyleToTitle(table, cell, style)
			 * RenderBeginTag(writer)
			 * RenderBeginTag(writer)
			 * "<tr>"
			 * -> The next/previous months things
			 * GetCalendarText("previousMonth", PrevMonthText, NextPrevStyle.ForeColor, isActive)
			 * RenderCalendarCell(writer, cell, ^^^)
			 * ..
			 * ..
			 * Then for NextMonthText
			 * "</tr>"
			 * "</tr>"
			 */
		}
		
		private void ApplyStyleToTitle(Table table, TableCell cell, TableItemStyle style)
		{
			throw new NotImplementedException();
			//TODO: Implement me
			/*
			 * Font
			 * Background color
			 * Foreground color
			 * Border color
			 * Border width
			 * Border style
			 * Vertical alignment
			 */
		}
		
		private void RenderCalendarCell(HtmlTextWriter writer, TableCell cell, string text)
		{
			cell.RenderBeginTag(writer);
			writer.Write(text);
			cell.RenderEndTag(writer);
		}
		
		private DateTime GetFirstCalendarDay(DateTime visibleDate)
		{
			DayOfWeek firstDay = ( FirstDayOfWeek == FirstDayOfWeek.Default ? DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek : FirstDayOfWeek);
			//FIXME: is (int)(Enum) correct?
			int days = (int)globCal.GetDayOfWeek(visibleDate) - (int)firstDay;
			if(days < 0)
			{
				days += 7;
			}
			return globCal.AddDays(visibleDate, -days);
		}
		
		private DateTime GetEffectiveVisibleDate()
		{
			DateTime dt = VisibleDate;
			if(dt.Equals(DateTime.MinValue))
			{
				dt = TodaysDate;
			}
			return new DateTime(globCal.GetYear(dt), globCal.GetMonth(dt), globCal.GetDayOfMonth(dt), globCal);
		}
		
		/// <summary>
		/// Creates text to be displayed, with all attributes if to be
		/// shown as a hyperlink
		/// </summary>
		private string GetCalendarText(string eventArg, string text, Color foreground, bool isLink)
		{
			if(isLink)
			{
				StringBuilder dispVal = new StringBuilder();
				dispVal.Append("<a href=\"");
				dispVal.Append(Page.GetPostBackClientHyperlink(this, eventArg));
				dispVal.Append("\" style=\"color: ");
				if(foreground.IsEmpty)
				{
					dispVal.Append(ColorTranslator.ToHtml(defaultTextColor));
				} else
				{
					dispVal.Append(ColorTranslator.ToHtml(foreground));
				}
				dispVal.Append("\">");
				dispVal.Append(text);
				dispVal.Append("</a>");
				return dispVal.ToString();
			}
			return text;
		}

		private string GetHtmlForCell(TableCell cell, bool showLinks)
		{
			StringWriter sw = new StringWriter();
			HtmlTextWriter htw = new HtmlTextWriter(sw);
			RenderBeginTag(htw);
			if(showLinks)
			{
				//sw.Write(GetCalendarText(,,ForeColor, true));
				//TODO: Implement me
			}
			throw new NotImplementedException();
		}
		
		internal DateTime SelectRangeInternal(DateTime fromDate, DateTime toDate, DateTime visibleDate)
		{
			TimeSpan span = fromDate - toDate;
			if(SelectedDates.Count != span.Days || SelectedDates[SelectedDate.Count - 1]!= toDate)
			{
				SelectedDates.SelectRange(fromDate, toDate);
				OnSelectionChanged();
			}
			if(globCal.GetMonth(fromDate) == globCal.GetMonth(fromDate) && globCal.GetMonth(fromDate) != globCal.GetMonth(visibleDate)
			{
				VisibleDate = new DateTime(globCal.GetYear(fromDate), globCal.getMonth(fromDate), 1, globCal);
				OnVisibleMonthChanged(VisibleDate, visibleDate);
			}
		}
	}
}
